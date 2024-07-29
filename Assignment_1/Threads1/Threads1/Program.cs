using System;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        // Create a new thread and pass in the method to be executed
        Thread backgroundThread = new Thread(new ThreadStart(BackgroundTask));

        // Start the thread
        backgroundThread.Start();

        // Continue with the main thread's work
        Console.WriteLine("Main thread is doing some work...");

        // Wait for the background thread to complete before exiting
        backgroundThread.Join();

        Console.WriteLine("Background task completed. Main thread ending.");
    }

    static void BackgroundTask()
    {
        Console.WriteLine("Background task started.");

        // Simulate some work
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"Background task is running... {i}");
            Thread.Sleep(1000); // Sleep for 1 second
        }

        Console.WriteLine("Background task finished.");
    }
}
