﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DictionaryLoader
{
    public abstract class Loader
    {
        protected string fInputFileName;
        public Loader(string inputFileName) {
            if (string.IsNullOrEmpty(inputFileName))
                throw new ArgumentNullException("fileName");
            fInputFileName = inputFileName;
        }
        public abstract LoadResult GetFromConfig();
    }
    public class XmlLoader : Loader
    {
        public XmlLoader(string inputFileName) : base(inputFileName) { }
        public override LoadResult GetFromConfig() {
            LoadResult result = new LoadResult();
            XmlDocument latAlphabet = new XmlDocument();
            try {
                latAlphabet.Load(fInputFileName);
            } catch (XmlException e) {
                result.Error = e;
            } catch (Exception ex) {
                result.Error = ex;
            }
            if (result.Error == null) {
                XmlElement letters = latAlphabet.DocumentElement;
                foreach (XmlNode letter in letters) {
                    XmlNode rus = letter.Attributes["rus"];
                    XmlNode lat = letter.Attributes["lat"];
                    result.Data.Add(rus.Value, lat.Value);
                }
            }
            return result;
        }
    }
    public class TextLoader : Loader {
        public TextLoader(string inputFileName) : base(inputFileName) { }
        public override LoadResult GetFromConfig() {
            LoadResult result = new LoadResult();
            string[] alphabet = new string[] { };
            try {
                alphabet = File.ReadAllLines(fInputFileName);
            } catch (FileNotFoundException e) {
                result.Error = e;
            }
            if (result.Error == null) {
                foreach (string pair in alphabet) {
                    string[] letters = pair.Split('|');
                    result.Data.Add(letters[0], letters[1]);
                }
            }
            return result;
        }
    }
    public class LoadResult {
        public Dictionary<string, string> Data { get; set; }
        public Exception Error { get; set; }
        public LoadResult() {
            Data = new Dictionary<string, string>();
        }
    }
}
