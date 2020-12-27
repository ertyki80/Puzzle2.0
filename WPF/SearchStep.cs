using System.Collections.Generic;

namespace WPF
{
    public class SearchStep
    {
        public SearchStep(IEnumerable<PiecePlacement> piecePlacements)
        {
            PiecePlacements = piecePlacements;
        }

        public IEnumerable<PiecePlacement> PiecePlacements { get; private set; }
    }
}
