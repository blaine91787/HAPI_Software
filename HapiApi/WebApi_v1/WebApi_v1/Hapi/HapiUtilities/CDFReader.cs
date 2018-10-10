using CDF;
using SPDF.CDF.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WebApi_v1.HAPI.Utilities
{
    public class CDFReader
    {
        private string DocPath;
        public CDF_File Doc { get; set; }
        public List<CDF_Attribute> Attributes { get; set; }
        public List<CDF_Variable> Variables { get; set; }


        public CDFReader(string cdfFilePath)
        {
            if (Doc != null)
                Doc.Close();
            DocPath = null;
            Doc = null;
            Attributes = null;
            Variables = null;
            if (Path.GetExtension(cdfFilePath) == ".cdf" && File.Exists(cdfFilePath))
            {
                DocPath = cdfFilePath;
                try { Doc = new CDF_File(DocPath, showExceptions: true); }
                catch (Exception exc) { throw exc; }
                Attributes = Doc.Attributes;
                Variables = Doc.Variables;

                //if (Doc.Attributes.Count == 0 || Doc.Variables.Count == 0)
            }
            else
            {
                throw new FileNotFoundException(""); // TODO: Handle Exception
            }
        }

        public void Close()
        {
            Doc.Close();
        }



        public List<CDF_Variable> GetListOfVariables(string[] variableNames)
        {
            List<CDF_Variable> temp = new List<CDF_Variable>();

            foreach (CDF_Variable var in Variables)
            {
                if (variableNames.Count() == 0)
                {
                    temp.Add(var);
                }
                else if (variableNames.Contains(var.Name.Trim().ToLower()))
                {
                    temp.Add(var);
                }
                //if(Array..Contains(var.Name.Trim().ToLower()))
                //    {

                //}
                //temp.Add(GetVariable(varName));
            }

            return temp;
        }

        private void InitHapiRecords(DateTime start, DateTime stop, string[] parameters, out List<int> indeces, out string[] headers)
        {
            List<int> indecesTemp = new List<int>();
            List<string> headersTemp = new List<string>();
            CDF_Variable utcVar = Doc.GetVariable("UTC");

            Converters con = new Converters();
            for (int i = 0; i < utcVar.Records; i++)
            {
                DateTime utc = con.ConvertUTCtoDate(utcVar[i].ToString());
                if (utc >= start && utc < stop)
                    indecesTemp.Add(i);
            }

            List<CDF_Variable> vars = GetListOfVariables(parameters);

            foreach (CDF_Variable var in vars)
            {
                headersTemp.Add(var.Name);
            }

            indeces = indecesTemp;
            headers = headersTemp.ToArray();
        }

        public IEnumerable<Dictionary<string,string>> GetCDFHapiRecords(DateTime start, DateTime stop, string[] parameters)
        {
            List<Dictionary<string, string>> records = new List<Dictionary<string, string>>();
            List<int> indeces;
            string[] header;
            //List<string> product;
            InitHapiRecords(start, stop, parameters, out indeces, out header);

            List<CDF_Variable> varList = GetListOfVariables(parameters);
            for (int i = 0; i < indeces.ToArray().Length; i++)
            {
                Dictionary<string, string> temp = new Dictionary<string, string>();
                foreach (CDF_Variable var in varList)
                {
                    string str = GetVariableString(var, indeces[i]);
                    temp.Add(var.Name, str);
                }
                records.Add(temp);
            }
            return records;
        }










        public List<CDF_Variable> GetVariableList()
        {
            if (Doc == null)
                return null;

            return Doc.Variables;
        }
        public List<CDF_Attribute> GetCDFAttributes()
        {
            if (Doc == null)
                return null;

            return Doc.Attributes;
        }
        public CDF_Variable GetVariable(string varName)
        {
            if (Doc == null)
                return null;

            if (Doc.Variables.Count == 0)
                return default(CDF_Variable);

            foreach (CDF_Variable var in Variables)
            {
                if (var.Name == varName)
                    return var;
            }

            return null;
        }

        public string GetVariableString(CDF_Variable var, int rec)
        {
            // If it's nonvarying there's only one record, so just set rec to 0
            if(var.Variance == CDF_Variances.NOVARY)
            {
                rec = 0;
            }
            if (var[rec] == null)
            {
                return var.GetPRBEMFillValue().ToString();
            }

            Type valueType = var[rec].GetType();

            //Debug.WriteLine(valueType.ToString());

            string valString = "";
            if (valueType.Equals(typeof(string))) { valString = var[rec].ToString(); }
            else if (valueType.Equals(typeof(string[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(string[,]))) { TwoDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(Double))) { valString = var[rec].ToString(); }
            else if (valueType.Equals(typeof(Double[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(Double[,]))) { TwoDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(Int32))) { valString = var[rec].ToString(); }
            else if (valueType.Equals(typeof(Int32[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(Int32[,]))) { TwoDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(UInt32))) { valString = var[rec].ToString(); }
            else if (valueType.Equals(typeof(UInt32[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(UInt32[,]))) { TwoDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(Single))) { valString = var[rec].ToString(); }
            else if (valueType.Equals(typeof(Single[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(Single[,]))) { TwoDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(Int16))) { valString = var[rec].ToString(); }
            else if (valueType.Equals(typeof(Int16[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(Int16[,]))) { TwoDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(SByte))) { valString = var[rec].ToString(); }
            else if (valueType.Equals(typeof(SByte[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(SByte[,]))) { TwoDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(CDF_Time))) { valString = var[rec].ToString(); }
            else if (valueType.Equals(typeof(CDF_Time[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
            else if (valueType.Equals(typeof(CDF_Time[,]))) { TwoDimCDFVariable(var[rec], valueType, out valString); }
            else { throw new ArgumentOutOfRangeException(valueType.Name); }


            return valString.Trim(new char[] { ',', ' ' });
        }
        public void GetVariables()
        {
            if (Path.GetExtension(DocPath) != ".cdf" || !File.Exists(DocPath))
                throw new FileNotFoundException("No valid cdf path available.");

            CDF_File cdfFile = new CDF_File(DocPath);

            int numrecs = cdfFile.GetVariable("Epoch").RecordsInFile;
            for (int rec = 0; rec < numrecs; rec++)
            {
                foreach (CDF_Variable var in cdfFile)
                {
                    if (var[rec] == null) { Console.WriteLine("THE NULL VARIABLE IS : " + var.Name); continue; }

                    Type valueType = var[rec].GetType();

                    //Debug.WriteLine(valueType.ToString());

                    string valString = "";
                    if (valueType.Equals(typeof(string))) { valString = var[rec].ToString(); }
                    else if (valueType.Equals(typeof(string[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
                    else if (valueType.Equals(typeof(string[,]))) { Debug.WriteLine("It's a String[,]"); }
                    else if (valueType.Equals(typeof(Double))) { valString = var[rec].ToString(); }
                    else if (valueType.Equals(typeof(Double[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
                    else if (valueType.Equals(typeof(Double[,]))) { TwoDimCDFVariable(var[rec], valueType, out valString); }
                    else if (valueType.Equals(typeof(Int32))) { valString = var[rec].ToString(); }
                    else if (valueType.Equals(typeof(Int32[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
                    else if (valueType.Equals(typeof(Int32[,]))) { Debug.WriteLine("It's a Int32[,]"); }
                    else if (valueType.Equals(typeof(UInt32))) { valString = var[rec].ToString(); }
                    else if (valueType.Equals(typeof(UInt32[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
                    else if (valueType.Equals(typeof(UInt32[,]))) { Debug.WriteLine("It's a UInt32[,]"); }
                    else if (valueType.Equals(typeof(Single))) { valString = var[rec].ToString(); }
                    else if (valueType.Equals(typeof(Single[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
                    else if (valueType.Equals(typeof(Single[,]))) { Debug.WriteLine("It's a Single[,]"); }
                    else if (valueType.Equals(typeof(Int16))) { valString = var[rec].ToString(); }
                    else if (valueType.Equals(typeof(Int16[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
                    else if (valueType.Equals(typeof(Int16[,]))) { Debug.WriteLine("It's a Int16[,]"); }
                    else if (valueType.Equals(typeof(SByte))) { valString = var[rec].ToString(); }
                    else if (valueType.Equals(typeof(SByte[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
                    else if (valueType.Equals(typeof(SByte[,]))) { Debug.WriteLine("It's a SByte[,]"); }
                    else if (valueType.Equals(typeof(CDF_Time))) { valString = var[rec].ToString(); }
                    else if (valueType.Equals(typeof(CDF_Time[]))) { OneDimCDFVariable(var[rec], valueType, out valString); }
                    else if (valueType.Equals(typeof(CDF_Time[,]))) { Debug.WriteLine("It's a CDF_Time[,]"); }
                    else { throw new ArgumentOutOfRangeException(valueType.Name); }
                    Type type = typeof(Double[,]);

                    Console.WriteLine(valString);
                }
            }

            cdfFile.Close();
        }
        public void GetAttributes()
        {
            if (Path.GetExtension(DocPath) != ".cdf" || !File.Exists(DocPath))
                throw new FileNotFoundException("No valid cdf path available.");

            CDF_File cdfFile = new CDF_File(DocPath);

            //Debug.WriteLine("SHOWING CDF FILE ATTRIBUTES:\n");
            foreach (CDF_Attribute attr in cdfFile.Attributes)
            {
                //Debug.WriteLine(attr.Name + "    :    " + attr.GetValue(0,-1));
            }

            //Debug.WriteLine("SHOWING CDF VARIABLE ATTRIBUTES:\n");
            foreach (CDF_Variable var in cdfFile.Variables)
            {
                foreach (CDF_Attribute attr in var.Attributes)
                {
                    //Double value = 0.0;
                    //Single[,] s = new Single[,] { { Single.MinValue }, { Single.MinValue } };
                    //if (var.WindowsType == typeof(Single)) { s = (Single[,])var[0]; value = Convert.ToDouble(s[0, 0]); }
                    //if (s[0, 0] == value) { //Debug.WriteLine("MATCHESSSSSSSS"); }

                    ////Debug.WriteLine(var.Name + "  :  " +
                    //                //value.ToString() + "   :   " +
                    //                //s[0, 0].ToString() + "  :  " +
                    //                var[0].ToString() + "  :  " +
                    //                var.WindowsType + "," + 
                    //                var.DataType + "," + 
                    //                var.VarType + " : " + 
                    //                var.Size +"    :    " + 
                    //                attr.Name + "   :   " + 
                    //                attr.GetValue(0,var));
                }
                Debug.WriteLine("");
            }
        }
        private void OneDimCDFVariable(Object varRec, Type type, out string valString)
        {
            if (!(type == typeof(String[])
               || type == typeof(Double[])
               || type == typeof(Int32[])
               || type == typeof(UInt32[])
               || type == typeof(Single[])
               || type == typeof(Int16[])
               || type == typeof(SByte[])
               || type == typeof(CDF_Time[])))
            {
                throw new ArgumentOutOfRangeException("type", type, "Not a valid one dimensional variable.");
            }

            StringBuilder sb = new StringBuilder();

            // Using dynamic variable because it can be any of the above types. The process is the same.
            dynamic valArr = varRec;
            string temp;
            sb.Append("[");
            foreach(var val in valArr)
            {
                sb.AppendFormat("{0},", val.ToString().Trim());
            }
            temp = sb.ToString().TrimEnd(',');
            sb.Clear();
            sb.Append(temp);
            sb.Append("]");
            valString = sb.ToString().Trim(new char[] { ',', ' ' });
        }
        private void TwoDimCDFVariable(Object varRec, Type type, out string valString)
        {
            if (!(type == typeof(String[,]) 
               || type == typeof(Double[,]) 
               || type == typeof(Int32[,]) 
               || type == typeof(UInt32[,]) 
               || type == typeof(Single[,]) 
               || type == typeof(Int16[,]) 
               || type == typeof(SByte[,]) 
               || type == typeof(CDF_Time[,])))
            {
                throw new ArgumentOutOfRangeException("type", type, "Not a valid two dimensional variable.");
            }

            StringBuilder sb = new StringBuilder();

            // Using dynamic variable because it can be any of the above types. The process is the same.
            dynamic valArr = varRec;
            string temp;
            sb.Append("[");
            for (int i = 0; i < valArr.GetLength(0); i++)
            {
                sb.Append("[");
                for (int j = 0; j < valArr.GetLength(1); j++)
                {
                    sb.AppendFormat("{0},", valArr[i, j]);
                }
                temp = sb.ToString().TrimEnd(',');
                sb.Clear();
                sb.Append(temp);
                sb.Append("],");
            }
            temp = sb.ToString().TrimEnd(',');
            sb.Clear();
            sb.Append(temp);
            sb.Append("]");

            valString = sb.ToString().Trim(new char[] { ',', ' ' });
        }
    }
}