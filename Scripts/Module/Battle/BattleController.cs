using System.Collections.Generic;
using System.Linq;
using kemocard.Scripts.Card;
using kemocard.Scripts.Common;
using kemocard.Scripts.Module.Run;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.Controller;
using kemocard.Scripts.Pawn;
using Vector2 = Godot.Vector2;

namespace kemocard.Scripts.Module.Battle;

public class BattleController : BaseController
{
    public BattleSystem BattleSystem = new();

    public BattleController()
    {
        GameCore.ViewMgr.Register(ViewType.BattleView, new ViewInfo
        {
            ViewName = "BattleView",
            ViewType = ViewType.BattleView,
            Controller = this,
            ResPath = GameCore.GetScenePath("BattleView"),
        });

        BattleModel model = new BattleModel(this);
        SetModel(model);

        InitModuleEvent();
    }

    public sealed override void InitModuleEvent()
    {
        base.InitModuleEvent();
        RegisterEvent(CommonEvent.StartBattle, OnBattleBegin);
        RegisterEvent(CommonEvent.BattleEvent_UseCard, OnBattleEventUseCard);
        RegisterEvent(CommonEvent.BattleEvent_CancelUseCard, OnBattleEventCancelUseCard);
        RegisterEvent(CommonEvent.BattleEvent_StartTurn, OnBattleEventStartTurn);
        RegisterEvent(CommonEvent.StartBattle_BY_PRESET, OnStartBattleByPreset);
        RegisterEvent(CommonEvent.BattleEvent_EndTurn, OnTurnEnd);
    }

    private void OnBattleEventStartTurn(object[] obj)
    {
        (Model as BattleModel)?.OnTurnStart();
    }

    private void OnBattleEventCancelUseCard(object[] obj)
    {
        (Model as BattleModel)?.OnCancelBattleUseCard(obj[0] as BaseBattleCard);
    }

    private void OnBattleEventUseCard(object[] obj)
    {
        (Model as BattleModel)?.OnBattleUseCard(obj[0] as BaseBattleCard);
    }

    private void OnBattleBegin(object[] obj)
    {
        var model = GetModel<BattleModel>();
        if (model == null) return;
        if (obj[1] is not List<BasePawn> enemies || enemies.Count == 0)
        {
            StaticUtil.ShowBannerHint("没有可战斗的敌人");
            return;
        }

        List<BaseCharacter> team = [];
        if (obj[0] == null)
        {
            var runController = GameCore.ControllerMgr.GetModule<RunController>(ControllerType.Run);
            team = runController.GetModel<RunModel>()?.Team.Values.ToList();
        }
        else if (obj[0] is List<BaseCharacter> tlist)
        {
            team = tlist;
        }

        if (team == null || team.Count == 0 || team.All(t => t == null) ||
            team.All(t => string.IsNullOrWhiteSpace(t.Id)))
        {
            StaticUtil.ShowBannerHint("没有可战斗的英雄");
            return;
        }

        GameCore.ViewMgr.OpenView(ViewType.BattleView, obj);
        model.Init();
        model.OnBattleStart(team, obj[1] as List<BasePawn>);
    }

    private void OnStartBattleByPreset(object[] obj)
    {
        string presetId;
        if (obj[0] is string str)
        {
            presetId = str;
        }
        else
        {
            StaticUtil.ShowBannerHint("战斗启动的参数错误");
            return;
        }

        var conf = GameCore.Tables.TbBattlePreset.GetOrDefault(presetId);
        if (conf == null)
        {
            StaticUtil.ShowBannerHint("战斗预设不存在");
            return;
        }

        List<BasePawn> enemies = [];
        for (var index = 0; index < conf.Monsterlist.Count; index++)
        {
            var enemyId = conf.Monsterlist[index];
            var enemy = new BasePawn();
            enemy.InitFromConfig(enemyId);
            enemy.Position = new Vector2
            {
                X = conf.MonsterPos.Count > index ? conf.MonsterPos[index].X : 0,
                Y = conf.MonsterPos.Count > index ? conf.MonsterPos[index].Y : 0,
            };
            enemies.Add(enemy);
        }

        OnBattleBegin([null, enemies]);
    }

    private void OnTurnEnd(object[] obj)
    {
        (Model as BattleModel)?.OnTurnEnd();
    }
}