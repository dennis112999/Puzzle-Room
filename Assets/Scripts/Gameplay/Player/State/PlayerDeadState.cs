using DG.Tweening;
using UnityEngine;

using Tools.ServiceLocator;
using Services.AudioService;

namespace PuzzleRoom.Gameplay
{
    public sealed class PlayerDeadState : IState
    {
        private readonly Player _p;

        public PlayerDeadState(Player player)
        {
            _p = player;
        }

        public void Enter()
        {
            ServicesManager.GetService<IAudioService>().PlaySE(SESoundData.SE.Wrong);

            Sequence seq = DOTween.Sequence();
            seq.Append(_p.transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental));
            seq.Join(_p.transform.DOScale(Vector3.zero, 2f).SetEase(Ease.InBack));

            GameController.Instance.EnterGameLose();
        }

        public void Tick() { }
        public void FixedTick() { }
        public void Exit() { }
    }
}
