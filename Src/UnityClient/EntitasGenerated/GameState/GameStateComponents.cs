//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameStateContext {

    public GameStateEntity levelEntity { get { return GetGroup(GameStateMatcher.Level).GetSingleEntity(); } }
    public Entitas.Data.LevelComponent level { get { return levelEntity.level; } }
    public bool hasLevel { get { return levelEntity != null; } }

    public GameStateEntity SetLevel(int newValue) {
        if (hasLevel) {
            throw new Entitas.EntitasException("Could not set Level!\n" + this + " already has an entity with Entitas.Data.LevelComponent!",
                "You should check if the context already has a levelEntity before setting it or use context.ReplaceLevel().");
        }
        var entity = CreateEntity();
        entity.AddLevel(newValue);
        return entity;
    }

    public void ReplaceLevel(int newValue) {
        var entity = levelEntity;
        if (entity == null) {
            entity = SetLevel(newValue);
        } else {
            entity.ReplaceLevel(newValue);
        }
    }

    public void RemoveLevel() {
        levelEntity.Destroy();
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameStateContext {

    public GameStateEntity loadingProgressEntity { get { return GetGroup(GameStateMatcher.LoadingProgress).GetSingleEntity(); } }
    public Entitas.Data.LoadingProgressComponent loadingProgress { get { return loadingProgressEntity.loadingProgress; } }
    public bool hasLoadingProgress { get { return loadingProgressEntity != null; } }

    public GameStateEntity SetLoadingProgress(float newValue) {
        if (hasLoadingProgress) {
            throw new Entitas.EntitasException("Could not set LoadingProgress!\n" + this + " already has an entity with Entitas.Data.LoadingProgressComponent!",
                "You should check if the context already has a loadingProgressEntity before setting it or use context.ReplaceLoadingProgress().");
        }
        var entity = CreateEntity();
        entity.AddLoadingProgress(newValue);
        return entity;
    }

    public void ReplaceLoadingProgress(float newValue) {
        var entity = loadingProgressEntity;
        if (entity == null) {
            entity = SetLoadingProgress(newValue);
        } else {
            entity.ReplaceLoadingProgress(newValue);
        }
    }

    public void RemoveLoadingProgress() {
        loadingProgressEntity.Destroy();
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameStateContext {

    public GameStateEntity nextSceneNameEntity { get { return GetGroup(GameStateMatcher.NextSceneName).GetSingleEntity(); } }
    public Entitas.Data.NextSceneNameComponent nextSceneName { get { return nextSceneNameEntity.nextSceneName; } }
    public bool hasNextSceneName { get { return nextSceneNameEntity != null; } }

    public GameStateEntity SetNextSceneName(string newValue) {
        if (hasNextSceneName) {
            throw new Entitas.EntitasException("Could not set NextSceneName!\n" + this + " already has an entity with Entitas.Data.NextSceneNameComponent!",
                "You should check if the context already has a nextSceneNameEntity before setting it or use context.ReplaceNextSceneName().");
        }
        var entity = CreateEntity();
        entity.AddNextSceneName(newValue);
        return entity;
    }

    public void ReplaceNextSceneName(string newValue) {
        var entity = nextSceneNameEntity;
        if (entity == null) {
            entity = SetNextSceneName(newValue);
        } else {
            entity.ReplaceNextSceneName(newValue);
        }
    }

    public void RemoveNextSceneName() {
        nextSceneNameEntity.Destroy();
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameStateEntity {

    public Entitas.Data.LevelComponent level { get { return (Entitas.Data.LevelComponent)GetComponent(GameStateComponentsLookup.Level); } }
    public bool hasLevel { get { return HasComponent(GameStateComponentsLookup.Level); } }

    public void AddLevel(int newValue) {
        var index = GameStateComponentsLookup.Level;
        var component = CreateComponent<Entitas.Data.LevelComponent>(index);
        component.Value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceLevel(int newValue) {
        var index = GameStateComponentsLookup.Level;
        var component = CreateComponent<Entitas.Data.LevelComponent>(index);
        component.Value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveLevel() {
        RemoveComponent(GameStateComponentsLookup.Level);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameStateEntity {

    public Entitas.Data.LoadingProgressComponent loadingProgress { get { return (Entitas.Data.LoadingProgressComponent)GetComponent(GameStateComponentsLookup.LoadingProgress); } }
    public bool hasLoadingProgress { get { return HasComponent(GameStateComponentsLookup.LoadingProgress); } }

    public void AddLoadingProgress(float newValue) {
        var index = GameStateComponentsLookup.LoadingProgress;
        var component = CreateComponent<Entitas.Data.LoadingProgressComponent>(index);
        component.Value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceLoadingProgress(float newValue) {
        var index = GameStateComponentsLookup.LoadingProgress;
        var component = CreateComponent<Entitas.Data.LoadingProgressComponent>(index);
        component.Value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveLoadingProgress() {
        RemoveComponent(GameStateComponentsLookup.LoadingProgress);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameStateEntity {

    public Entitas.Data.NextSceneNameComponent nextSceneName { get { return (Entitas.Data.NextSceneNameComponent)GetComponent(GameStateComponentsLookup.NextSceneName); } }
    public bool hasNextSceneName { get { return HasComponent(GameStateComponentsLookup.NextSceneName); } }

    public void AddNextSceneName(string newValue) {
        var index = GameStateComponentsLookup.NextSceneName;
        var component = CreateComponent<Entitas.Data.NextSceneNameComponent>(index);
        component.Value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceNextSceneName(string newValue) {
        var index = GameStateComponentsLookup.NextSceneName;
        var component = CreateComponent<Entitas.Data.NextSceneNameComponent>(index);
        component.Value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveNextSceneName() {
        RemoveComponent(GameStateComponentsLookup.NextSceneName);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameStateMatcher {

    static Entitas.IMatcher<GameStateEntity> _matcherLevel;

    public static Entitas.IMatcher<GameStateEntity> Level {
        get {
            if (_matcherLevel == null) {
                var matcher = (Entitas.Matcher<GameStateEntity>)Entitas.Matcher<GameStateEntity>.AllOf(GameStateComponentsLookup.Level);
                matcher.componentNames = GameStateComponentsLookup.componentNames;
                _matcherLevel = matcher;
            }

            return _matcherLevel;
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameStateMatcher {

    static Entitas.IMatcher<GameStateEntity> _matcherLoadingProgress;

    public static Entitas.IMatcher<GameStateEntity> LoadingProgress {
        get {
            if (_matcherLoadingProgress == null) {
                var matcher = (Entitas.Matcher<GameStateEntity>)Entitas.Matcher<GameStateEntity>.AllOf(GameStateComponentsLookup.LoadingProgress);
                matcher.componentNames = GameStateComponentsLookup.componentNames;
                _matcherLoadingProgress = matcher;
            }

            return _matcherLoadingProgress;
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameStateMatcher {

    static Entitas.IMatcher<GameStateEntity> _matcherNextSceneName;

    public static Entitas.IMatcher<GameStateEntity> NextSceneName {
        get {
            if (_matcherNextSceneName == null) {
                var matcher = (Entitas.Matcher<GameStateEntity>)Entitas.Matcher<GameStateEntity>.AllOf(GameStateComponentsLookup.NextSceneName);
                matcher.componentNames = GameStateComponentsLookup.componentNames;
                _matcherNextSceneName = matcher;
            }

            return _matcherNextSceneName;
        }
    }
}
