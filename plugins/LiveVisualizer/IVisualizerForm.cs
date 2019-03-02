using System.Collections.Generic;

namespace LiveVisualizer
{
    public interface IVisualizerForm
    {
        void SetStrains(IList<double> strains);

        void SetBackgroundImage(string imageLocation);

        void SetPp(double pp);

        void SetHits(int h300, int h100, int h50);

        void Show();

        void Hide();

    }
}