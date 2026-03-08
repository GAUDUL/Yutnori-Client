public class MapEventEntry
{
    public IMapEvent Event { get; set; }
    public int Probability { get; set; }

    public MapEventEntry(IMapEvent ev, int prob)
    {
        Event = ev;
        Probability = prob;
    }
}