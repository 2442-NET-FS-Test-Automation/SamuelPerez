namsespace DsaThreading;

public static class Searches
{
    public static int LinearSearch(int[] data, int targert)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == target) return i;
        }
        return -1;
    }
    
    public static int BinarySearch(int[] sorted, int target)
    {
        
    }

}
