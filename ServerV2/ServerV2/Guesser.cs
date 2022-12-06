using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerV2
{
    internal abstract class Guesser
    {   
        protected Guesser(int a)
        {
            _neverGuess = a;
        }
        public readonly int _neverGuess;
        public abstract bool IsGuessed();
        public abstract bool IsGuessRight(int guess);
        public abstract int GetHiddenNumber();
        public abstract int GetClosest();
        public abstract int GetAttempts();
    }

    internal class RandGuesser : Guesser
    {
        private readonly int Target;
        private int Closest;
        public int attempts;
        public RandGuesser(int min = 0, int max = 2000000, int neverGuessAssignetValueToClosest = -1) : base(neverGuessAssignetValueToClosest)
        {
            Target = new Random().Next(min, max);
            Closest = neverGuessAssignetValueToClosest;
            attempts = 0;
        }
        public override bool IsGuessed()
        {
            return Closest == Target;
        }
        public override bool IsGuessRight(int guess) //if guesed 
        {
            if (guess < 0) { throw new ArgumentException("Guess is less than zero"); }

            attempts++;
            if (Target == guess)
            {
                Closest = Target;
                return true;
            }
            else
            {
                if (Math.Abs(Target - Closest) > Math.Abs(Target - guess))
                {
                    Closest = guess;
                }

                return false;
            }
        }
        public override int GetHiddenNumber()
        {
            return Target;
        }
        public override int GetClosest()
        {
            return Closest;
        }
        public override int GetAttempts()
        {
            return attempts;
        }
    }
}
