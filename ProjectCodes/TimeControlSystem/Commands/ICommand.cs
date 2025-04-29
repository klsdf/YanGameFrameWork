/// <summary>
/// 基础命令接口
/// </summary>
public interface ICommand
{
    void Execute();
    void Stop();
    bool IsCompleted { get; }
}