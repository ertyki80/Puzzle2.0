using System;

namespace DL
{
    public class SolutionFoundEventArgs : EventArgs
    {
        internal SolutionFoundEventArgs(Solution solution, int solutionIndex)
        {
            Solution = solution;
            SolutionIndex = solutionIndex;
        }

        public Solution Solution { get; }

        public int SolutionIndex { get; }
    }
}
