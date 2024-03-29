﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WebApi_v1.HAPI.Catalog
{
    public class Product
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string HapiId { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string StartTime { get; set; }
        public string StopTime { get; set; }
        public Dictionary<string, Field> Fields { get; set; }



        public void GetProduct(XmlElement productElement, string basepath)
        {
            if (productElement == null)
                throw new ArgumentNullException("XmlElement is null.");

            if (!Directory.Exists(basepath))
                throw new ArgumentOutOfRangeException("basepath directory does not exist");

            if (!productElement.HasAttributes)
                throw new InvalidOperationException("Products must have attributes for catalog entry to be valid. Check the catalog xml for errors.");


            foreach (XmlAttribute attr in productElement.Attributes)
            {
                switch (attr.Name)
                {
                    case ("name"):
                        Name = attr.Value;
                        break;
                    case ("id"):
                        Id = attr.Value;
                        break;
                    case ("hapiId"):
                        HapiId = attr.Value;
                        break;
                    case ("title"):
                        Title = attr.Value;
                        break;
                    case ("path"):
                        Path = attr.Value;
                        if (Path.Contains("$data$"))
                        {
                            Path = Path.Replace("$data$", basepath).Replace(@"\\", @"\");
                            basepath = Path;
                        }
                        break;
                    case ("description"):
                        Description = attr.Value;
                        break;
                    case ("filetype"):
                        FileType = attr.Value;
                        break;
                    case ("starttime"):
                        StartTime = attr.Value;
                        break;
                    case ("stoptime"):
                        StopTime = attr.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(attr.Name, "Not a valid field attribute.");
                }
            }

            XmlNodeList fieldNodes = productElement.ChildNodes;
            Fields = new Dictionary<string, Field>();
            foreach (XmlNode fieldNode in fieldNodes)
            {
                Field field = new Field();
                if (fieldNode.GetType() == typeof(XmlElement))
                {
                    field.GetField((XmlElement)fieldNode);
                }
                Fields.Add(field.Name, field);
            }
        }

        public List<Field> GetFields()
        {
            List<Field> temp = new List<Field>();
            foreach(Field field in Fields.Values)
            {
                temp.Add(field);
            }
            return temp;
        }
        public override string ToString()
        {
            return String.Format(
                "Name:\t\t{0}\nId:\t\t{1}\nHapiId:\t\t{2}\nTitle:\t\t{3}\nPath:\t\t{4}\nDescription:\t{5}\n",
                this.Name,
                this.Id,
                this.HapiId,
                this.Title,
                this.Path,
                this.Description
            );
        }
    }
}
