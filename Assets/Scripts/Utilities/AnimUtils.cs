using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace FirebirdGames.Utilities
{
    public static class AnimUtils 
    {
        public static CanvasGroup GetOrAddCanvasGroup(GameObject target)
        {
            var mCanvasGroup = target.GetComponent<CanvasGroup>();
            if (mCanvasGroup == null)
                mCanvasGroup = target.AddComponent<CanvasGroup>();

            return mCanvasGroup;
        }

		public static float PlayAllTweensWithID(string tweenID)
		{
			var tweens = DOTween.TweensById(tweenID);

			if (tweens != null)
			{
				float maxTime = 0f;

				foreach (var tween in tweens)
				{
					float time = tween.Duration() + tween.Delay();
					if (time > maxTime)
						maxTime = time;
					if (tween.playedOnce)
						tween.Restart();
					else
						tween.Play();
				}

				return maxTime;
			}

			return 0f;
		}

		public static float GetTotalLengthOfChildTweensWithID(GameObject parent, string tweenID, bool includeInactive = false)
		{
			var tweenAnims = parent.GetComponentsInChildren<DOTweenAnimation>(includeInactive);

			float maxTime = 0f;

			foreach (var doTweenAnimation in tweenAnims.Where(tween => tween.id == tweenID && (includeInactive || tween.isActiveAndEnabled)))
			{
				float time = doTweenAnimation.duration + doTweenAnimation.delay;
				if (time > maxTime)
					maxTime = time;
			}

			return maxTime;
		}

		public static float GetTotalLengthOfTweensWithID(string tweenID)
		{
			var tweens = DOTween.TweensById(tweenID);

			if (tweens != null)
			{
				float maxTime = 0f;

				foreach (var tween in tweens)
				{
					float time = tween.Duration() + tween.Delay();
					if (time > maxTime)
						maxTime = time;
				}

				return maxTime;
			}

			return 0f;
		}

		public static float PlayAllChildTweensWithID(GameObject parent, string tweenID, bool includeInactive = false)
		{
			var tweenAnims = parent.GetComponentsInChildren<DOTweenAnimation>(includeInactive);

			float maxTime = 0f;

			foreach (var doTweenAnimation in tweenAnims.Where(tween => tween.id == tweenID && (includeInactive || tween.isActiveAndEnabled)))
			{
				float time = doTweenAnimation.duration + doTweenAnimation.delay;
				if (time > maxTime)
					maxTime = time;
				if (doTweenAnimation.tween == null)
					doTweenAnimation.CreateTween();

				if (doTweenAnimation.tween != null)
				{
					if (doTweenAnimation.tween.playedOnce)
						doTweenAnimation.tween.Restart();
					else
						doTweenAnimation.tween.Play();
				}
				else
				{
					Debug.LogWarning($"Tween Null: {doTweenAnimation.gameObject.name}");
				}
			}

			return maxTime;
		}

		//NOT WORKING???
		public static Sequence PlayAllChildTweensWithIdSeq(GameObject parent, string tweenID)
		{
			var seq = DOTween.Sequence();
			var tweenAnims = parent.GetComponentsInChildren<DOTweenAnimation>();

			float maxTime = 0f;

			foreach (var doTweenAnimation in tweenAnims)
			{
				if (doTweenAnimation.id == tweenID)
				{
					float time = doTweenAnimation.duration + doTweenAnimation.delay;
					if (time > maxTime)
						maxTime = time;
					doTweenAnimation.CreateTween();
					seq.Join(doTweenAnimation.tween); ;
				}
			}

			return seq;
		}

		public static float PlayAllChildTweens(GameObject parent)
		{
			var tweenAnims = parent.GetComponentsInChildren<DOTweenAnimation>();

			float maxTime = 0f;

			foreach (var doTweenAnimation in tweenAnims)
			{
				float time = doTweenAnimation.duration + doTweenAnimation.delay;
				if (time > maxTime)
					maxTime = time;
				doTweenAnimation.CreateTween();

				if (doTweenAnimation.tween.playedOnce)
					doTweenAnimation.tween.Restart();
				else
					doTweenAnimation.tween.Play();
			}

			return maxTime;
		}

		public static bool KillAllTweensWithID(string id)
		{
			var activeTweens = DOTween.TweensById(id);
			if (activeTweens != null)
			{
				foreach (var activeTween in activeTweens)
				{
					activeTween.Kill();
				}

				return true;
			}

			return false;
		}

		public static bool KillAllChildTweensWithID(GameObject parent, string id, bool complete = false)
		{
			var activeTweens = parent.GetComponentsInChildren<DOTweenAnimation>(false).Select(tw => tw.tween);
			if (activeTweens != null)
			{
				foreach (var tween in activeTweens)
				{
					tween.Kill(complete);
				}

				return true;
			}

			return false;
		}

		public static Sequence ShakeRotation(this Transform transform, float maxAngle, float animTime, int reverbs = -1)
        {
			var seq = DOTween.Sequence();
			float startAngle = transform.eulerAngles.z;

			if (reverbs > -1)
			{
				float animInterval = animTime / (reverbs * 2);
				for (int i = 0; i < reverbs; i++)
				{
					seq.Append(transform.DOLocalRotate(new Vector3(0, 0, startAngle - maxAngle), animInterval).SetEase(Ease.InOutQuad))
						.Append(transform.DOLocalRotate(new Vector3(0, 0, startAngle + maxAngle), animInterval).SetEase(Ease.InOutQuad));
				}

				seq.Append(transform.DOLocalRotate(new Vector3(0, 0, startAngle), animInterval / 2f).SetEase(Ease.InOutQuad));
			}
			else
            {
				seq.Append(transform.DOLocalRotate(new Vector3(0, 0, startAngle - maxAngle), animTime/2f).SetEase(Ease.InOutQuad))
						.Append(transform.DOLocalRotate(new Vector3(0, 0, startAngle + maxAngle), animTime/2f).SetEase(Ease.InOutQuad));

				seq.SetLoops(-1, LoopType.Yoyo);
			}

			return seq;
        }
		
		public static Sequence Wobble(GameObject target, float angleExtent, float animTime, int reverbs = 3, float delay = 0f)
		{
			float animInterval = animTime / (reverbs * 2);
			var seq = DOTween.Sequence();
			var targetTrans = target.transform;
			float startAngleZ = target.transform.localEulerAngles.z;

			for(int i = 0; i < reverbs; i++)
			{
				seq.Append(targetTrans.DOLocalRotate(new Vector3(0, 0, startAngleZ - angleExtent), animInterval).SetEase(Ease.InOutQuad))
					.Append(targetTrans.DOLocalRotate(new Vector3(0, 0, startAngleZ + angleExtent), animInterval).SetEase(Ease.InOutQuad));
			}

			seq.Append(targetTrans.DOLocalRotate(new Vector3(0, 0, startAngleZ), animInterval/2f).SetEase(Ease.InOutQuad));
			seq.SetDelay(delay);
			seq.SetTarget(targetTrans);

			return seq;
		}
	}
}
