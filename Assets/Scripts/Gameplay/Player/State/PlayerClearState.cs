using DG.Tweening;

using UnityEngine;
using Tools.ServiceLocator;

using Services.EventBus;
using PuzzleRoom.Event;
using Services.AudioService;

namespace PuzzleRoom.Gameplay
{
    public sealed class PlayerClearState : IState
    {
        private readonly Player _p;

        private Vector3 _goalPos;

        public PlayerClearState(Player player)
        {
            _p = player;
        }

        public void SetGoal(Vector3 goalPos)
        {
            _goalPos = goalPos;
        }

        public void Enter()
        {
            _p.Rb.velocity = Vector2.zero;
            _p.Rb.simulated = false;

            ServicesManager.GetService<IAudioService>().PlaySE(SESoundData.SE.Puzzle);

            PlayClearAnimation();
        }

        public void Tick() { }
        public void FixedTick() { }

        public void Exit() { }

        private void PlayClearAnimation()
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(_p.transform.DOMove(_goalPos, 1f));

            seq.Join(_p.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.LocalAxisAdd));
            seq.Join(_p.transform.DOScale(Vector3.zero, 1f));

            seq.OnComplete(() => {
                SpriteRenderer spriteRenderer = _p.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    seq.Join(spriteRenderer.DOFade(0f, 1f));
                }

                ServicesManager.GetService<IEventBus>().Publish<RequestLoadStageEvent>(new RequestLoadStageEvent(_p.StageId));
            });
        }
    }
}
