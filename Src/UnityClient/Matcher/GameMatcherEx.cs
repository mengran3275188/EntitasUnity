using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherMainPlayerPosition;
    public static Entitas.IMatcher<GameEntity> MainPlayerPosition
    {
        get
        {
            if(_matcherMainPlayerPosition == null)
            {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.MainPlayer, GameComponentsLookup.Position);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherMainPlayerPosition = matcher;
            }
            return _matcherMainPlayerPosition;
        }
    }
}
