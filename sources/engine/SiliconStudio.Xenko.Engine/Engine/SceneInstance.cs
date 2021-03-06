// Copyright (c) 2014-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SiliconStudio.Core;
using SiliconStudio.Core.Collections;
using SiliconStudio.Core.Reflection;
using SiliconStudio.Core.Serialization.Contents;
using SiliconStudio.Xenko.Engine.Design;
using SiliconStudio.Xenko.Rendering;

namespace SiliconStudio.Xenko.Engine
{
    /// <summary>
    /// Manage a collection of entities within a <see cref="RootScene"/>.
    /// </summary>
    public sealed class SceneInstance : EntityManager
    {
        /// <summary>
        /// A property key to get the current scene from the <see cref="RenderContext.Tags"/>.
        /// </summary>
        public static readonly PropertyKey<SceneInstance> Current = new PropertyKey<SceneInstance>("SceneInstance.Current", typeof(SceneInstance));

        /// <summary>
        /// A property key to get the current render system from the <see cref="RenderContext.Tags"/>.
        /// </summary>
        public static readonly PropertyKey<RenderSystem> CurrentRenderSystem = new PropertyKey<RenderSystem>("SceneInstance.CurrentRenderSystem", typeof(SceneInstance));

        /// <summary>
        /// A property key to get the current visibility group from the <see cref="RenderContext.Tags"/>.
        /// </summary>
        public static readonly PropertyKey<VisibilityGroup> CurrentVisibilityGroup = new PropertyKey<VisibilityGroup>("SceneInstance.CurrentVisibilityGroup", typeof(SceneInstance));

        private readonly Dictionary<TypeInfo, RegisteredRenderProcessors> registeredRenderProcessorTypes = new Dictionary<TypeInfo, RegisteredRenderProcessors>();

        private Scene rootScene;

        public TrackingCollection<VisibilityGroup> VisibilityGroups { get; }

        /// <summary>
        /// Occurs when the scene changed from a scene child component.
        /// </summary>
        public event EventHandler<EventArgs> RootSceneChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityManager" /> class.
        /// </summary>
        /// <param name="registry">The registry.</param>
        public SceneInstance(IServiceRegistry registry) : this(registry, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneInstance" /> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="rootScene">The scene entity root.</param>
        /// <param name="executionMode">The mode that determines which processors are executed.</param>
        /// <exception cref="System.ArgumentNullException">services
        /// or
        /// rootScene</exception>
        public SceneInstance(IServiceRegistry services, Scene rootScene, ExecutionMode executionMode = ExecutionMode.Runtime) : base(services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            ExecutionMode = executionMode;
            VisibilityGroups = new TrackingCollection<VisibilityGroup>();
            VisibilityGroups.CollectionChanged += VisibilityGroups_CollectionChanged;

            RootScene = rootScene;
        }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        public Scene RootScene
        {
            get { return rootScene; }
            set
            {
                if (rootScene == value)
                    return;

                if (rootScene != null)
                {
                    Remove(rootScene);
                    RemoveRendererTypes();
                }

                if (value != null)
                {
                    Add(value);
                    HandleRendererTypes();
                }

                rootScene = value;
                OnRootSceneChanged();
            }
        }

        protected override void Destroy()
        {
            RootScene = null;

            // Cleaning processors should not be necessary anymore, but physics are not properly cleaned up otherwise
            Reset();

            // TODO: Dispose of Scene, graphics compositor...etc.
            // Currently in Destroy(), not sure if we should clear that list on Reset() as well?
            VisibilityGroups.Clear();

            base.Destroy();
        }

        /// <summary>
        /// Gets the current scene valid only from a rendering context. May be null.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>SiliconStudio.Xenko.Engine.SceneInstance.</returns>
        public static SceneInstance GetCurrent(RenderContext context)
        {
            return context.Tags.Get(Current);
        }

        private void Add(Scene scene)
        {
            foreach (var entity in scene.Entities)
                Add(entity);

            // Listen to future changes in Scene.Entities
            foreach (var childScene in scene.Children)
                Add(childScene);

            // Listen to future changes in entities and child scenes
            scene.Children.CollectionChanged += Children_CollectionChanged;
            scene.Entities.CollectionChanged += Entities_CollectionChanged;
        }

        private void Remove(Scene scene)
        {
            scene.Entities.CollectionChanged -= Entities_CollectionChanged;
            scene.Children.CollectionChanged -= Children_CollectionChanged;

            foreach (var childScene in scene.Children)
                Remove(childScene);

            foreach (var entity in scene.Entities)
                Remove(entity);
        }

        private void Entities_CollectionChanged(object sender, TrackingCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add((Entity)e.Item);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove((Entity)e.Item);
                    break;
            }
        }

        private void Children_CollectionChanged(object sender, TrackingCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add((Scene)e.Item);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove((Scene)e.Item);
                    break;
            }
        }

        private void HandleRendererTypes()
        {
            foreach (var componentType in ComponentTypes)
            {
                EntitySystemOnComponentTypeAdded(null, componentType);
            }
            ComponentTypeAdded += EntitySystemOnComponentTypeAdded;
        }

        private void RemoveRendererTypes()
        {
            // Unregister render processors
            ComponentTypeAdded -= EntitySystemOnComponentTypeAdded;
            foreach (var renderProcessors in registeredRenderProcessorTypes)
            {
                foreach (var renderProcessorInstance in renderProcessors.Value.Instances)
                {
                    Processors.Remove(renderProcessorInstance.Value);
                }
            }
            registeredRenderProcessorTypes.Clear();
        }

        private void VisibilityGroups_CollectionChanged(object sender, TrackingCollectionChangedEventArgs e)
        {
            var visibilityGroup = (VisibilityGroup)e.Item;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var registeredRenderProcessorType in registeredRenderProcessorTypes)
                    {
                        var processor = CreateRenderProcessor(registeredRenderProcessorType.Value, visibilityGroup);

                        // Assume we are in middle of a compositor draw so we need to run it manually once (Update/Draw already happened)
                        var renderContext = RenderContext.GetShared(Services);
                        processor.Update(renderContext.Time);
                        processor.Draw(renderContext);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var registeredRenderProcessorType in registeredRenderProcessorTypes)
                    {
                        // Remove matching entries
                        var instances = registeredRenderProcessorType.Value.Instances;
                        for (int i = 0; i < instances.Count; ++i)
                        {
                            var registeredProcessorInstance = instances[i];
                            if (registeredProcessorInstance.Key == visibilityGroup)
                            {
                                Processors.Remove(registeredProcessorInstance.Value);
                                instances.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    visibilityGroup.Dispose();
                    break;
            }
        }

        private void EntitySystemOnComponentTypeAdded(object sender, TypeInfo type)
        {
            var rendererTypeAttributes = type.GetCustomAttributes<DefaultEntityComponentRendererAttribute>();
            foreach (var rendererTypeAttribute in rendererTypeAttributes)
            {
                var processorType = AssemblyRegistry.GetType(rendererTypeAttribute.TypeName);
                if (processorType == null || !typeof(IEntityComponentRenderProcessor).GetTypeInfo().IsAssignableFrom(processorType.GetTypeInfo()))
                {
                    continue;
                }

                var registeredProcessors = new RegisteredRenderProcessors(processorType, VisibilityGroups.Count);
                registeredRenderProcessorTypes.Add(type, registeredProcessors);

                // Create a render processor for each visibility group
                foreach (var visibilityGroup in VisibilityGroups)
                {
                    CreateRenderProcessor(registeredProcessors, visibilityGroup);
                }
            }
        }

        private EntityProcessor CreateRenderProcessor(RegisteredRenderProcessors registeredRenderProcessor, VisibilityGroup visibilityGroup)
        {
            // Create
            var processor = (EntityProcessor)Activator.CreateInstance(registeredRenderProcessor.Type);

            // Set visibility group
            ((IEntityComponentRenderProcessor)processor).VisibilityGroup = visibilityGroup;

            // Add processor
            Processors.Add(processor);
            registeredRenderProcessor.Instances.Add(new KeyValuePair<VisibilityGroup, EntityProcessor>(visibilityGroup, processor));

            return processor;
        }

        private void OnRootSceneChanged()
        {
            RootSceneChanged?.Invoke(this, EventArgs.Empty);
        }

        private class RegisteredRenderProcessors
        {
            public Type Type;
            public FastListStruct<KeyValuePair<VisibilityGroup, EntityProcessor>> Instances;

            public RegisteredRenderProcessors(Type type, int instanceCount)
            {
                Type = type;
                Instances = new FastListStruct<KeyValuePair<VisibilityGroup, EntityProcessor>>(instanceCount);
            }
        }
    }
}
