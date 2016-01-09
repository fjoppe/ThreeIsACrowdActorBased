# Three Is A Crowd Actor Based
Actor based version of Three is a Crowd

This project continues where Three is a Crowd left off. 

The game logic was already working, and now it is time to introduce multi-player features to the game. 

The game needs to be played with three players, and these players arrive at an undefined frequency. 
For this, a player first enters a waiting room. When the waiting room has 3 players, they continue to a game room.
When it takes too long for new players to arrive, AI players will enter the waiting-room. 

This system is to be build with Akka.Net, Akkling and Wire.

Goal of this project is not only to implement a waiting room system. Another goal is to experiment with
design patterns, in order to learn more about Akka.Net.
