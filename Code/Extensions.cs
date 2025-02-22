using Godot;
using System.Threading.Tasks;

public static class SignalAwaiterExtensions 
{
    public static async Task AsTask(this SignalAwaiter awaiter) => await awaiter;
}
