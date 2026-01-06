using System.Threading;
using System.Threading.Tasks;

public interface IAnimator
{
    Task PlayAsync(string anim, CancellationToken ct);
    void Play(string anim);
}

public interface IDamagePopup
{
    Task ShowAsync(int amount, CancellationToken ct);
}

public interface IClock { Task DelayMs(int ms, CancellationToken ct); }
