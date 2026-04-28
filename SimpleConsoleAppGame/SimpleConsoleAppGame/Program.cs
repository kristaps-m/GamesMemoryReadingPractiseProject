//Console.WriteLine("Hello, World!");

//var playerHealth = 100;

//while (true)
//{
//    Console.WriteLine(playerHealth);
//    Thread.Sleep(3000);
//}

// ChatGPT v.1.0.0
//using System.Runtime.InteropServices;

//Console.WriteLine("Game started!");

//GCHandle handle;

//var player = new Player();
//player.Health = 100;

//// Pin object in memory
//handle = GCHandle.Alloc(player, GCHandleType.Pinned);

//Console.WriteLine($"PID: {Environment.ProcessId}");

//while (true)
//{
//    Console.WriteLine($"Health: {player.Health}");
//    Thread.Sleep(3000);
//}

//class Player
//{
//    public int Health;
//}

// CoPilot Claude Haiku 4.5 v.1.0.0
using System.Runtime.InteropServices;

Console.WriteLine("Game started!");

GCHandle handle;

var player = new Player();
player.Health = 1000;
player.Mana = 333;
player.X = 123.45f; player.Y = 67.89f;

// Pin object in memory so it doesn't move
handle = GCHandle.Alloc(player, GCHandleType.Pinned);
IntPtr playerAddress = handle.AddrOfPinnedObject();

Console.WriteLine($"PID: {Environment.ProcessId}");
Console.WriteLine($"Player Address: 0x{playerAddress:X}");

int updateCount = 0;
while (true)
{
    // Simulate health changes
    player.Health = 1000 - (updateCount % 50);
    player.Mana = 333 - (updateCount % 10);
    Console.WriteLine($"Health: {player.Health} // Mana: {player.Mana} // X: {player.X} // Y: {player.Y}");
    Thread.Sleep(3000);
    updateCount++;
}

class Player
{
    public int Health;
    public int Mana;
    public float X;
    public float Y;
}

// ChatGPT v.1.0.1
//using System.Runtime.InteropServices;

//int[] values = new int[1];
//values[0] = 100;

//GCHandle handle = GCHandle.Alloc(values, GCHandleType.Pinned);

//IntPtr address = handle.AddrOfPinnedObject();

//Console.WriteLine($"PID: {Environment.ProcessId}");
//Console.WriteLine($"Address: 0x{address.ToString("X")}");

//while (true)
//{
//    Console.WriteLine(values[0]);
//    Thread.Sleep(3000);
//}