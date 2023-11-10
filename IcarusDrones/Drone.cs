using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IcarusDrones
{
    //6.1	Create a separate class file to hold the data items of the Drone.
    //Use separate getter and setter methods, ensure the attributes are private and the accessor methods are public.
    //Add a display method that returns a string for Client Name and Service Cost.
    //Add suitable code to the Client Name and Service Problem accessor methods so the data is formatted as Title case or Sentence case.
    //Save the class as “Drone.cs”.
    class Drone
    {
        // Global variables
        private string _name;
        private string _model;
        private string _problem;
        private double _cost;
        private int _tag;

        // Contstructor
        public Drone(string name, string model, string problem, double cost, int tag)
        {
            _name = name;
            _model = model;
            _problem = problem;
            _cost = cost;
            _tag = tag;
        }

        // Convert to title case
        private string ToTitleCase(string input)
        {
            // Create a TextInfo
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            return textInfo.ToTitleCase(input);
        }

        // Convert to sentence case
        private string ToSentenceCase(string input)
        {
            var lowerCase = input.ToLower();

            var r = new Regex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture);
            return r.Replace(lowerCase, s => s.Value.ToUpper());
        }

        // Get / Set Name
        public string GetName()
        { return ToTitleCase(_name); }
        public void SetName(string name)
        { _name = name; }

        // Get / Set Model
        public string GetModel()
        { return _model; }
        public void SetModel(string model)
        { _model = model; }

        // Get / Set Problem
        public string GetProblem()
        { return ToSentenceCase(_problem); }
        public void SetProblem(string problem)
        { _problem = problem; }

        // Get / Set Cost
        public double GetCost()
        { return _cost; }
        public void SetCost(double cost)
        { _cost = cost; }

        // Get / Set Tag
        public int GetTag()
        { return _tag; }
        public void SetTag(int tag)
        { _tag = tag; }
    }
}
