using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class TestClass
{

    private static int[] RandomizeIndex(int numLevels, int seed)
    {
        int[] randomizedIndex;

        System.Random random = new System.Random(seed);
        randomizedIndex = new int[numLevels];
        for (int index = 0; index < randomizedIndex.Length; ++index)
          randomizedIndex[index] = index;

        for (int index1 = 0; index1 < randomizedIndex.Length; ++index1)
        {
          int index2 = random.Next(0, randomizedIndex.Length);
          int num = randomizedIndex[index1];
          randomizedIndex[index1] = randomizedIndex[index2];
          randomizedIndex[index2] = num;
        }
        return randomizedIndex;
    }

    static void Main(string[] args)
    {
        Dictionary<string, int> rush_levels = new Dictionary<string, int>();
        rush_levels.Add("White", 96);
        rush_levels.Add("white", 96);
        rush_levels.Add("Mikey", 96);
        rush_levels.Add("mikey", 96);
        rush_levels.Add("Yellow", 8);
        rush_levels.Add("yellow", 8);
        rush_levels.Add("Violet", 8);
        rush_levels.Add("violet", 8);
        rush_levels.Add("Red",    8);
        rush_levels.Add("red",    8);  

        Dictionary<string, int> bool_strs = new Dictionary<string, int>();
        bool_strs.Add("True", 1);
        bool_strs.Add("true", 1);
        bool_strs.Add("False", 0);
        bool_strs.Add("false", 0);

        Dictionary<int, string> idx_to_levelMappings = new Dictionary<int, string>();
        Dictionary<string, int> level_to_idxMappings = new Dictionary<string, int>();
        
        string[] level_names = System.IO.File.ReadAllLines("clean_levelnames.txt");
        for (int i = 0; i < level_names.Length; ++i)
        {
            idx_to_levelMappings[i] = level_names[i];
            level_to_idxMappings[level_names[i].ToLower()] = i;
        }
        
        if (args.Length == 3)
        {
            if (args[0] == "-g" || args[0] == "--generate")
            {   
                int num_Levels;
                try
                {
                    num_Levels = rush_levels[args[1]];
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    Console.WriteLine("Unknown rush name: " + args[0]);
                    return;
                }
                if (num_Levels != 96)
                {
                    Console.WriteLine("Only 96 level rushes are supported for now.");
                    return;
                }
                int seed = int.Parse(args[2]);
                int[] randomized_Index = RandomizeIndex(num_Levels, seed);
                File.WriteAllLines("new_order.txt", randomized_Index.Select(x => idx_to_levelMappings[x]).ToArray());
            }
            return;
        }

        if (args.Length != 4)
        {
            Console.WriteLine("Usage:\n\tfind_seed.exe <rush_name> <order_matters> <start_sequence> <level_depth>");
            Console.WriteLine("\tfind_seed.exe -g <rush_name> <seed>");
            Console.WriteLine("");
            Console.WriteLine("\trush_name: (White|Mikey|Yellow|Violet|Red) name of rush, case insensitive");
            Console.WriteLine("\torder_matters: (True|False) True if you care about the exact order of the starting levels (this may result in very long computation for large level sequences)");
            Console.WriteLine("\tstart_sequence: A comma-separated list (no spaces) of levels in the desired starting sequence (1-indexed or as strings, case insensitive)");
            Console.WriteLine("\tlevel_depth: (int) for a non-ordered sequence, the number levels into the run to search for the desired levels");
            Console.WriteLine("\tseed: (int) seed to generate order from");
            Console.WriteLine("");
            Console.WriteLine("\texample: find_seed.exe White False \"The Third Temple,absolution,The Clocktower\" 3");
            Console.WriteLine("\t\tThis will find the seed that plays White's rush, with the first 3 levels being the boss levels of the rush");
            Console.WriteLine("\t\t(it should be 58685, with starting sequence of Absolution,The Clocktower,The Third Temple,Pop,Shield,Cleaner,...)");
            Console.WriteLine("");
            Console.WriteLine("\texample: find_seed.exe -g White 87921");
            Console.WriteLine("\t\tThis will generate the sequence of levels for seed 87921 and write it to new_order.txt");
            Console.WriteLine("");
            Console.WriteLine("\texample: find_seed.exe yellow True 8,7,6,5,4,3,2,1 8");
            Console.WriteLine("\t\tThis will find the seed that plays the 8 level rushes backwards (it should be 121166)");
            Console.WriteLine("");
            Console.WriteLine("\texample: mikey False \"The Third Temple,The Clocktower,Marathon,Breakthrough,absolution,congregation,canals\" 15");
            Console.WriteLine("\t\tThis will find the seed that plays Mikey's rush, with the first 15 levels containing the desired levels");
            Console.WriteLine("\t\tNote that there are only 7 levels being searched for, but the search depth is 15, so that the search will actually find a seed");
            
            
            
            return;
        }
       

        int numLevels;
        int orderMatters;

        try
        {
            numLevels = rush_levels[args[0]];
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            Console.WriteLine("Unknown rush name: " + args[0]);
            return;
        }

         try
        {
        orderMatters = bool_strs[args[1]];
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            Console.WriteLine("Unknown order matters value: " + args[1]);
            return;
        }
        
        int[] targetArray;
        int trash;
        
        if (int.TryParse(args[2].Split(',')[0], out trash))
        {
            targetArray = args[2].Split(',').Select(x => int.Parse(x)-1).ToArray(); // 0-index it
        }
        else
        {
            try
            {
            targetArray = args[2].Split(',').Select(x => level_to_idxMappings[x.ToLower()]).ToArray();
             }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                Console.WriteLine("Error: Unknown level name somewhere in: " + args[2]);
                return;
            }
        }

        int searchDepth;
        if (!int.TryParse(args[3], out searchDepth))
        {
            Console.WriteLine("Error: search depth must be an integer");
            return;
        }
        if (searchDepth < targetArray.Length)
        {
            searchDepth = targetArray.Length;
        }
        
        int ranseed = 1;

        HashSet<int> targetSet = new HashSet<int>(targetArray);

        int[] randomizedIndex;
        while (true)
        {
            randomizedIndex = RandomizeIndex(numLevels, ranseed);
            if (orderMatters == 1)
            {
                var startArray = randomizedIndex.Take(targetArray.Length).ToArray();
                if (Enumerable.SequenceEqual(startArray,targetArray)){break;}

            }
            else
            {
                var startSet = new HashSet<int>(randomizedIndex.Take(searchDepth));
                if (startSet.IsSupersetOf(targetSet)) {break;}
            }
            
            if (ranseed == 2147483647) 
            {   
                Console.WriteLine("No seed found for the given search parameters");
                return;
            }

            ranseed++;
            if (ranseed % 20000000 == 0)
            {
                Console.WriteLine("Current Search Seed: " + ranseed);
            }
        }
        Console.WriteLine("Seed: " + ranseed);
        Console.WriteLine("SeedSequence: " + string.Join(",", randomizedIndex.Select(x => x+1)));   // 1-index it
        //Console.WriteLine("Names:\n" + string.Join("\n", randomizedIndex.Select(x => idx_to_levelMappings[x])));

        
    }
}