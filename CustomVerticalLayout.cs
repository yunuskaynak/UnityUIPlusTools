using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class CustomVerticalLayout : MonoBehaviour
{
    [Header("Move Animation")]
    [SerializeField] private float moveDuration = 0.3f;
    [SerializeField] private float moveDelay = 0.1f;
    [SerializeField] private float moveScale = 0.8f;
    [SerializeField] private float moveScaleDuration = 0.15f;
    [Header("Scale In/Out Animation")]
    [SerializeField] private float scaleInDuration = 0.2f;
    [SerializeField] private float scaleInDelay;
    [SerializeField] private float scaleOutDuration = 0.2f;
    [SerializeField] private float scaleOutDelay;
    [Header("Animation Ease (global)")]
    [SerializeField] private Ease animationEase = Ease.InOutSine;

    private readonly List<float> _slotPositionsY = new();
    private readonly List<RectTransform> _layoutChilds = new();

    private VerticalLayoutGroup _layoutGroup;
    private readonly Queue<Func<UniTask>> _animationQueue = new();
    private bool _isAnimating = false;
    private bool _isLocked = false;

    private void Awake()
    {
        _layoutGroup = GetComponent<VerticalLayoutGroup>();
    }

    private void Start()
    {
        RunStartup().Forget();

        async UniTaskVoid RunStartup()
        {
            try
            {
                InitChilds();
                await UniTask.Delay(500);
                InitSlotPositions();
                if (_layoutGroup) _layoutGroup.enabled = false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"CustomVerticalLayout Start Exception: {ex}");
            }
        }
    }

    private void InitChilds()
    {
        _layoutChilds.Clear();
        foreach (Transform t in transform)
        {
            var rect = t.GetComponent<RectTransform>();
            if (rect != null) _layoutChilds.Add(rect);
        }
    }

    private void InitSlotPositions()
    {
        _slotPositionsY.Clear();
        foreach (var rect in _layoutChilds)
            _slotPositionsY.Add(rect.anchoredPosition.y);
    }

    public void ActivateChild(RectTransform[] childRects, bool allAtOnce = true) => EnqueueAnimation(() => ActivateChildInternal(childRects, allAtOnce));
    public void DeactivateChild(RectTransform[] childRects, bool allAtOnce = true) => EnqueueAnimation(() => DeactivateChildInternal(childRects, allAtOnce));
    public void FillSpacesToTop() => EnqueueAnimation(FillSpacesToTopInternal);
    public void ResetToOriginalPositions() => EnqueueAnimation(SetToOriginalPositionsInternal);

    private void EnqueueAnimation(Func<UniTask> anim)
    {
        if (_isLocked)
            return;
        _animationQueue.Enqueue(anim);
        if (!_isAnimating)
        {
            _isAnimating = true;
            RunAnimationQueue().Forget();
        }
    }

    private async UniTaskVoid RunAnimationQueue()
    {
        _isLocked = true;
        try
        {
            while (_animationQueue.Count > 0)
                await _animationQueue.Dequeue()();
        }
        catch (Exception ex)
        {
            Debug.LogError($"CustomVerticalLayout Animation Exception: {ex}");
        }
        _isLocked = false;
        _isAnimating = false;
    }

    // ---- Animation Logic ----

    private async UniTask ActivateChildInternal(RectTransform[] childRects, bool allAtOnce)
    {
        // 1. Move already active objects (not being activated) to original slots
        var actives = new List<RectTransform>();
        foreach (var rect in _layoutChilds)
        {
            int targetIdx = _layoutChilds.IndexOf(rect);
            if (rect.gameObject.activeSelf && Array.IndexOf(childRects, rect) < 0 && rect.anchoredPosition.y != _slotPositionsY[targetIdx])
                actives.Add(rect);
        }

        if (allAtOnce)
        {
            var moves = new List<UniTask>();
            foreach (var rect in actives)
            {
                int idx = _layoutChilds.IndexOf(rect);
                moves.Add(AnimateMoveAndStretch(rect, _slotPositionsY[idx]));
            }
            await UniTask.WhenAll(moves);
        }
        else
        {
            // Move from last to first
            for (int i = actives.Count - 1; i >= 0; i--)
            {
                var rect = actives[i];
                int idx = _layoutChilds.IndexOf(rect);
                await AnimateMoveAndStretch(rect, _slotPositionsY[idx]);
            }
        }

        // 2. Activate all given objects together (scale-in in parallel, only if not already active)
        var toActivate = new List<UniTask>();
        foreach (var rect in childRects)
        {
            int idx = _layoutChilds.IndexOf(rect);
            if (rect && idx >= 0 && !rect.gameObject.activeSelf)
            {
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, _slotPositionsY[idx]);
                rect.localScale = Vector3.zero;
                rect.gameObject.SetActive(true);
                toActivate.Add(rect.DOScale(Vector3.one, scaleInDuration).SetEase(animationEase).AsyncWaitForCompletion().AsUniTask());
            }
        }
        await UniTask.WhenAll(toActivate);
        if (scaleInDelay > 0 && toActivate.Count > 0) await UniTask.Delay((int)(scaleInDelay * 1000));
    }

    private async UniTask DeactivateChildInternal(RectTransform[] childRects, bool allAtOnce)
    {
        var outs = new List<UniTask>();
        foreach (var rect in childRects)
            outs.Add(rect.DOScale(Vector3.zero, scaleOutDuration).SetEase(animationEase).AsyncWaitForCompletion().AsUniTask());
        await UniTask.WhenAll(outs);
        if (scaleOutDelay > 0 && childRects.Length > 0) await UniTask.Delay((int)(scaleOutDelay * 1000));
        foreach (var rect in childRects)
            rect.gameObject.SetActive(false);

        // 2. Fill spaces to top (all at once or one by one)
        var activeRects = new List<RectTransform>();
        foreach (var rect in _layoutChilds)
            if (rect.gameObject.activeSelf)
                activeRects.Add(rect);

        if (allAtOnce)
        {
            var moves = new List<UniTask>();
            for (int i = 0; i < activeRects.Count; i++)
                if (activeRects[i].anchoredPosition.y != _slotPositionsY[i])
                    moves.Add(AnimateMoveAndStretch(activeRects[i], _slotPositionsY[i]));
            await UniTask.WhenAll(moves);
        }
        else
        {
            for (int i = 0; i < activeRects.Count; i++)
                if (activeRects[i].anchoredPosition.y != _slotPositionsY[i])
                    await AnimateMoveAndStretch(activeRects[i], _slotPositionsY[i]);
        }
    }

    private async UniTask SetToOriginalPositionsInternal()
    {
        for (int i = 0; i < _layoutChilds.Count; i++)
        {
            var rect = _layoutChilds[i];
            if (rect && rect.gameObject.activeSelf && rect.anchoredPosition.y != _slotPositionsY[i])
            {
                await AnimateMoveAndStretch(rect, _slotPositionsY[i]);
            }
        }
    }

    private async UniTask FillSpacesToTopInternal()
    {
        int slotIndex = 0;
        foreach (var rect in _layoutChilds)
        {
            if (rect.gameObject.activeSelf)
            {
                if (rect.anchoredPosition.y != _slotPositionsY[slotIndex])
                {
                    await AnimateMoveAndStretch(rect, _slotPositionsY[slotIndex]);
                }
                slotIndex++;
            }
        }
    }

    private async UniTask AnimateMoveAndStretch(RectTransform rect, float toY)
    {
        if (rect.anchoredPosition.y == toY)
            return;
        await rect.DOScale(Vector3.one * moveScale, moveScaleDuration).SetEase(animationEase).AsyncWaitForCompletion().AsUniTask();
        await rect.DOAnchorPosY(toY, moveDuration).SetEase(animationEase).AsyncWaitForCompletion().AsUniTask();
        await rect.DOScale(Vector3.one, moveScaleDuration).SetEase(animationEase).AsyncWaitForCompletion().AsUniTask();
        if (moveDelay > 0) await UniTask.Delay((int)(moveDelay * 1000));
    }
}
