using CDF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SPDF.CDF.CSharp;

namespace ConsoleApp1
{

    public class Result
    {
        public string Path { get; set; }
        public string Exception { get; set; }
        public DateTime PrevModified { get; set; }
        public DateTime CurrModified { get; set; }
    }
    public class CDFTester
    {
        public string dir = String.Empty;
        public bool succ = false;
        public IntPtr _fileID = default(IntPtr);
        public int _curStatus;
        public int Status = -1;
        public bool _isOpen = false;
        public bool ExceptionThrown = false;
        public Exception CurrentException;
        public bool IgnoreExceptions = true;
        private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        public List<Thread> workerThreads = new List<Thread>();
        public List<Result> results = new List<Result>();
        public List<string> paths = new List<string>();
        //private static object locker = new Object();

        public CDFTester()
        {

        }

        public void Run(string[] paths)
        {
            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    string[] pathSplit = path.Split('\\');
                    dir = pathSplit[pathSplit.Count()-2] + pathSplit[pathSplit.Count()-1];
                    Tester(path);
                }
            }

            ThreadMaker();

            int count = 0;
            foreach (Thread th in workerThreads)
            {
                th.Start();
                //Console.WriteLine(count++);//"\n\n\n\n\n" + (count++) + "\n\n\n\n\n");
            }


            foreach (Thread th in workerThreads)
            {
                //Console.WriteLine(count--); //"\n\n\n\n\n" + (count--) + "\n\n\n\n\n");
                th.Join();
            }

            WriteToFile();
        }

        public void WriteToFile()
        {
            int logNum = 1;
            string logPath = @"C:\Users\blaine.harris\Desktop\INVALID CDFs";
            string path = logPath + @"\" + dir + "_";
            while (File.Exists(path + logNum + ".txt"))
                logNum++;

            logPath = path + logNum + ".txt";

            if (results.Count > 0)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(logPath))
                    {
                        foreach (Result result in results)
                        {
                            sw.WriteLine("############    " + result.Exception + "    ############");
                            sw.WriteLine(result.Path);
                            sw.WriteLine("Last modify before running:   " + result.PrevModified);
                            sw.WriteLine("Last modify after running:    " + result.CurrModified);
                            sw.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------");
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Unable to access file: " + logPath);
                }
            }
        }

        public void ThreadMaker()
        {
            foreach (string path in paths)
            {
                Thread th;
                //CDFTester test = new CDFTester();
                th = new Thread(delegate ()
                {
                    WorkerThread(path);
                });
                workerThreads.Add(th);
            }
        }
        public void Tester(string path)
        {
            if (Directory.GetDirectories(path).Count() > 0)
            {
                foreach (string dir in Directory.GetDirectories(path))
                {
                    Tester(dir);
                }
            }
            if (Directory.GetFiles(path, "*.cdf").Count() > 0)
                paths.Add(path);
        }

        public void WorkerThread(string path)
        {
            //ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
            foreach (string fileName in Directory.GetFiles(path))
            {
                FileInfo fi = new FileInfo(fileName);
                //if (path.ToLower().Contains("tofxeh") && path.ToLower().Contains("2013"))
                    //Console.WriteLine(fi.FullName);
                DateTime prevmod = fi.LastWriteTime;
                DateTime currmod = default(DateTime);
                if (fi.Extension == ".cdf")
                {

                    try
                    {
                        //Console.Write(".");

                        //locker.EnterWriteLock();
                        //CDF_File cdf = new CDF_File(fi.FullName);
                        //cdf.Close();

                        try
                        {
                            unsafe
                            {
                                void* id = null;
                                try
                                {
                                    
                                    locker.EnterWriteLock();
                                    _curStatus = CDFAPIs.CDFopenCDF(fi.FullName, &id);

                                }
                                catch (AccessViolationException e)
                                {
                                    //Text = "AccessViolationException unhandled from file " + FullName;
                                    throw e;
                                    //return succ;
                                }
                                _fileID = (IntPtr)id;
                            }
                            if ((Status == CDFConstants.CDF_OK) || (Status == CDFConstants.BACKWARD_)) _isOpen = true;
                            succ = true;
                        }
                        catch (CDFException exc)
                        {
                            throw exc;
                        }
                        //Console.WriteLine(fi.FullName);
                        //cdf = new CDFReader(fi.FullName);
                        try { unsafe { _curStatus = CDFAPIs.CDFcloseCDF((void*)_fileID); } }
                        catch (CDFException exc) { if (IgnoreExceptions) { ExceptionThrown = true; CurrentException = exc; } else throw; }

                        fi.Refresh();
                        currmod = fi.LastWriteTime;
                        if (prevmod != currmod)
                            throw new Exception("The modified dates are changing");
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "The modified dates are changing")
                            throw new Exception("The modified dates are changing");

                        fi.Refresh();
                        currmod = fi.LastWriteTime;

                        if (prevmod != currmod)
                            Debug.WriteLine("The modified dates are changing");

                        //locker.EnterWriteLock();
                        Result res = new Result
                        {
                            Path = fi.FullName,
                            Exception = e.Message,
                            PrevModified = prevmod,
                            CurrModified = currmod,
                        };

                        results.Add(res);

                        if (locker.IsReadLockHeld)
                            locker.ExitReadLock();
                        if (locker.IsWriteLockHeld)
                            locker.ExitWriteLock();
                    }
                    finally
                    {
                        fi.Refresh();
                        currmod = fi.LastWriteTime;

                        if (prevmod != currmod)
                            Debug.WriteLine("The modified dates are changing");

                        if (locker.IsReadLockHeld)
                            locker.ExitReadLock();
                        if (locker.IsWriteLockHeld)
                            locker.ExitWriteLock();
                    }
                }
            }
        }
    }
}
