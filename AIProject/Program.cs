using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Project
{
    private static readonly Random random = new Random();

    // Number of individuals in each generation, it can be change to see the effect
    private const int POPULATION_SIZE = 100;

    // Valid Genes 
    private const string GENES = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOP" +
                                  "QRSTUVWXYZ 1234567890, .-;:_!\"#%&/()=?@${[]}";

    // Target string to be generated 
    private const string TARGET = "(Generative AI)";

    private const double CROSSOVER_PROBABILITY = 0.7;

    private const double MUTATION_PROBABILITY = 0.01;

    public static void Main()
    {
        int genSum = 0;

        // will be executed 2 times 
        for (int i = 0; i < 2; i++)
        {
            long startTime = DateTime.Now.Ticks;

            /* Step 1 : Randomly initialize populations p*/
            List<string> population = CreateFirstGeneration(POPULATION_SIZE, TARGET.Length);
            int generationNum = 0;
            bool passFound = false;

            /* step 2 : determine fitness of population */
            SortPopulation(population);

            if (Fitness(population[0]) == 0)
            {
                passFound = true;
                Console.WriteLine("Password is found in the first try!" + population[0]);
            }

            /* Step 3: Until success repeat*/
            while (!passFound)
            {

                List<string> nextPopulation = new List<string>();
                string bestChromosome = population[0];
                nextPopulation.Add(bestChromosome);

                /* Step 3a: Select parents from population*/
                for (int k = 0; k < POPULATION_SIZE / 2; k++)
                {
                    string parent1 = Selection(population);
                    string parent2 = Selection(population);
                    while (parent1.Equals(parent2))
                    {
                        parent2 = Selection(population);
                    }

                    /* Step 3b: Crossover and generate new population*/
                    string[] children = CrossOver(parent1, parent2);
                    /* Perform mutation on new population */
                    children[0] = Mutation(children[0]);
                    children[1] = Mutation(children[1]);

                    nextPopulation.Add(children[0]);
                    nextPopulation.Add(children[1]);
                }

                population = nextPopulation;
                SortPopulation(population);

                bestChromosome = population[0];
                /* Step 4: Calculate fitness for new population */
                double bestFitnessValue = Fitness(bestChromosome);
                Console.WriteLine("Generation: " + generationNum + " Closest Password: " + bestChromosome +
                    " Fitness Value: " + (int)bestFitnessValue);

                if (bestFitnessValue == 0)
                {
                    Console.WriteLine("At try number: " + i + " password is found using " + POPULATION_SIZE + " chromosome on the " +
                        generationNum + "th generation. ");
                    genSum += generationNum;
                    passFound = true;
                    long endTime = DateTime.Now.Ticks;
                    long totalTime = (endTime - startTime) / TimeSpan.TicksPerMillisecond;
                    Console.WriteLine(POPULATION_SIZE + " chromosome is used " +
                        i + " decoding transaction lasted " + totalTime + " milliseconds. ");
                }
                else
                {
                    generationNum++;
                }
            }
        }
        Console.WriteLine($"Average decoding time : {genSum / 2.0:F2}" + " milliseconds");
    }

    public static List<string> CreateFirstGeneration(int PopulationSize, int GenSize)
    {
        List<string> population = new List<string>();

        for (int i = 0; i < GenSize; i++)
        {
            string chromosome = "";
            for (int j = 0; j < GenSize; j++)
            {
                chromosome += (char)(random.Next(95) + 32);
            }
            population.Add(chromosome);
        }
        return population;
    }

    public static double Fitness(string chromosome)
    {
        int difference = 0;

        for (int i = 0; i < chromosome.Length; i++)
        {
            if (chromosome[i] != TARGET[i]) difference++;
        }
        return difference;
    }

    public static string[] CrossOver(string chrom1, string chrom2)
    {
        string[] newChromosomes = new string[2];

        if (random.NextDouble() < CROSSOVER_PROBABILITY)
        {
            int crossOverIndex = random.Next(chrom1.Length);
            newChromosomes[0] = chrom1.Substring(0, crossOverIndex) + chrom2.Substring(crossOverIndex);
            newChromosomes[1] = chrom2.Substring(0, crossOverIndex) + chrom2.Substring(crossOverIndex);
        }
        else
        {
            newChromosomes[0] = chrom1;
            newChromosomes[1] = chrom2;
        }
        return newChromosomes;
    }

    public static string Mutation(string chromosome)
    {
        string newChromosome = "";

        for (int i = 0; i < chromosome.Length; i++)
        {
            if (random.NextDouble() < MUTATION_PROBABILITY)
            {
                newChromosome += (char)(random.Next(95) + 32);
            }
            else
            {
                newChromosome += chromosome[i];
            }
        }
        return newChromosome;
    }

    public static void SortPopulation(List<string> population)
    {
        for (int i = 0; i < population.Count - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < population.Count; j++)
            {
                if (Fitness(population[j]) < Fitness(population[minIndex]))
                {
                    minIndex = j;
                }
            }
            if (minIndex != i)
            {
                string temp = population[i];
                population[i] = population[minIndex];
                population[minIndex] = temp;
            }
        }
    }

    public static string Selection(List<string> population)
    {
        double totalFitness = 0;
        foreach (string chromosome in population)
        {
            totalFitness += 1 / Fitness(chromosome);
        }

        double spin = random.NextDouble() * totalFitness;
        double partialSum = 0;
        for (int i = 0; i < population.Count; i++)
        {
            partialSum += 1 / Fitness(population[i]);
            if (spin < partialSum)
            {
                return population[i];
            }
        }
        return population[population.Count - 1];
    }
}







