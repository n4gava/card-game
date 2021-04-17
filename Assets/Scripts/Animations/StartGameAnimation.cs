using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StartGameAnimation : Animation
{
    public Player player;
    public Player enemy;
    public Transform playerPortraitInitialPosition;
    public Transform enemyPortraitInitialPosition;
    public GameObject backgroundOverlay;
    public float portraitInitialScale = 2f;
    public string initialLayerPortrait = "AboveEverything";
    public Canvas startGameCanvas;

    public override Sequence AnimationSequence()
    {
        backgroundOverlay.SetActive(true);
        var sequence = DOTween.Sequence();
        sequence.Insert(0f, PortraitMoveToPosition(player, playerPortraitInitialPosition.position));
        sequence.Insert(0f, PortraitMoveToPosition(enemy, enemyPortraitInitialPosition.position));
        sequence.Insert(0f, PortraitScale(player));
        sequence.Insert(0f, PortraitScale(enemy));
        sequence.Insert(0f, ShowStartGameCanvas());

        if (backgroundOverlay != null)
            sequence.Insert(0f, ShowBackgroundOverlay());

        sequence.PrependInterval(3f);

        return sequence;
    }

    private Tweener ShowBackgroundOverlay()
    {
        backgroundOverlay.SetActive(true);

        return backgroundOverlay.GetComponentInChildren<Image>().DOFade(0, 1f).OnComplete(() =>
        {
            backgroundOverlay.SetActive(false);
        });
    }

    private Tweener PortraitMoveToPosition(Player player, Vector3 startPortraitPosition)
    {
        var portaibleManager = player.PlayerArea.Portrait.GetComponent<PlayerPortraitManager>();
        var portraitCanvas = portaibleManager.playerPortraitVisual.GetComponent<Canvas>();
        var portraitPosition = portaibleManager.transform.position;

        portaibleManager.transform.position = startPortraitPosition;
        var originalPlayerPortrait = portraitCanvas.sortingLayerName;
        portraitCanvas.sortingLayerName = initialLayerPortrait;

        return player.PlayerArea.Portrait.transform.DOMove(portraitPosition, 1f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            portraitCanvas.sortingLayerName = originalPlayerPortrait;
        });
    }

    private Tweener PortraitScale(Player player)
    {
        var portraitGameObject = player.PlayerArea.Portrait.gameObject;
        var portraitScale = portraitGameObject.transform.localScale;

        portraitGameObject.transform.localScale *= portraitInitialScale;

        return player.PlayerArea.Portrait.transform.DOScale(portraitScale, 1f).SetEase(Ease.OutCirc);
    }

    private Tween ShowStartGameCanvas()
    {
        startGameCanvas.gameObject.SetActive(true);
        var image = startGameCanvas.GetComponentInChildren<Image>();

        return image.DOFade(0f, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            startGameCanvas.gameObject.SetActive(false);
        });
    }
}
