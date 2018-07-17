using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public abstract class Animal
    {
        public string AnimalType { get; set; }
        public string Name { get; set; }
        public int NumOfLegs { get; set; }
        public int NumOfEyes { get; set; }
        public bool CanSpeak { get; set; }
    }

    public class Dog : Animal , IMover
    {
        #region Dog/Animal Implementation
        public string Color { get; set; }
        public Dog()
        {
            AnimalType = "Dog";
            NumOfLegs = 4;
            NumOfEyes = 2;
            CanSpeak = true;
            Name = "undefined";
            Color = "undefined";
        }
        public Dog(string name, string color)
        {
            AnimalType = "Dog";
            NumOfLegs = 4;
            NumOfEyes = 2;
            CanSpeak = true;
            Name = name;
            Color = color;
        }        
        public override string ToString()
        {
            string str = "";
            str += "\n"
                    + "Type of Animal: " + AnimalType + "\n"
                    + "Name: " + Name + "\n"
                    + "# of Legs: " + NumOfLegs + "\n"
                    + "# of Eyes: " + NumOfEyes + "\n"
                    + "Can speak: " + CanSpeak + "\n"
                    + "Color: " + Color + "\n"
                    + "Moving Direction: " + Direction + "\n"
                    + "Distance Moved: " + DistanceMoved + "\n\n";
                    
            return str;
        }
        #endregion

        #region IMover Implementation
        private int _distanceMoved = 0;
        private int _numOfStepsMoved = 0;
        private readonly int _speed = 5;

        public string Direction { get; set; } = "";
        public int Speed { get => _speed; }
        public int DistanceMoved {
            get => _distanceMoved;
            set => _distanceMoved += value; }
        public int NumOfStepsMoved
        {
            get => _numOfStepsMoved;
            set => _numOfStepsMoved += value;
        }
        public void Move(int numsteps, string dir)
        {
            Direction = dir;
            NumOfStepsMoved = numsteps;
            DistanceMoved = numsteps * Speed;
        }
        #endregion
    }
}
