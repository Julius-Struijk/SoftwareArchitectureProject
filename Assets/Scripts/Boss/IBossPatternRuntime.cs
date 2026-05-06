namespace CMGTSA.Boss
{
    public interface IBossPatternRuntime
    {
        void OnBegin(IBossContext ctx);
        void Tick(float dt, IBossContext ctx);
        bool IsFinished { get; }
    }
}
