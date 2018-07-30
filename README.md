# Locomotive Creatures
For the final project of my Artificial Intelligence course, I implemented a genetic algorithm that tries to make a population of creatures move as far to the right as possible, by use of locomotion. I tried to implement [Alan Zucconi's Evolutionary Computation](https://www.alanzucconi.com/2016/04/06/evolutionary-coputation-1/) to compare my implementaion with his. This project was created with Unity. 

An executable and binaries of the project is available. There are input fields users can specify to have varying effects on the outcome. The algorithm will end when the max number of generations has been met. An output file is placed in the executable directory, and contains the best, average, and worst distances of each generation.

Default values for input:

Population Size: 1000,
Max Generations: 100,
Evalution Time (Sec): 20,
Mutation Rate: 0.1

## Approach
Psuedo-code of genetic algorithm:
```
Generate random population
For generation 1 -> n
    Evalutate fitness of each creature
    Do selection
    Repopulate
```
Genetic Algorithms are generally easy to understand conceptually. For my approach, I wanted to introduce as much randomness and deversity as possible.

### Fitness
Fitness is determined by a creature's distance from the start, and its form. Form is a function made up from a creatures facing direction, how long it is laying on the ground, idling, and generally "leveled"/balanced.

### Selection
During selection, half the population is killed off. However, creatures with higher fitness scores most likely stay. It is semi-random due to some low scored creatures may having good genes to pass on.

### Repopulate
Each creature has an array of floats. These values are genes of each creature, and the array is like a DNA strand. To repopulate, 2 random creature, from the current population, are selected. 2-point crossover and mutation is done between the creatures. This results in 2 new creatures. The 2 randomly selected and 2 newly created creatures are added to a new population. The randomly selected ones are removed from the current to avoid overlaps. If the population size is an odd number, a creature is cloned and the clone is muatated.

## Resources
[Nature of Code (Chapter 9) by Daniel Shiffman](https://natureofcode.com/book/chapter-9-the-evolution-of-code/)

[Evolution by Keiwan Donyagard](keiwan.itch.io/evolution)

[Evolution Simulator by Cary Huan](www.openprocessing.org/sketch/377698)

[Genetic Algorithm Walkers by Rafael Matsunaga](rednuht.org/genetic_walkers/)

[Evolutionary Computation by Alan Zucconi](https://www.alanzucconi.com/2016/04/06/evolutionary-coputation-1/)