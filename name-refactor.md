# Names refactor

```bash
Collektive.Unity
├── Editor
│   ├── BackendBuilder.cs          → CollektiveBackendBuilder.cs   (prefix for clarity in Editor context)
│   ├── GenerateProto.cs           → ProtoCompiler.cs              (verb-noun, says what it does)
│   ├── ReadOnlyDrawer.cs          (keep)
│   └── unibo.collektive.unity.Editor.asmdef
├── Runtime
│   ├── Interop                    (was BackendWrapper — "Interop" is the standard term for FFI layers)
│   │   ├── CollektiveBindings.cs  (was CollektiveNative — "Bindings" says it's a mapping, "Native" is ambiguous)
│   │   └── CollektiveAgent.cs     (was CollektiveNode — see Agent rationale below)
│   ├── Core                       (was CollectiveNode — was a catch-all with a typo)
│   │   ├── Agent.cs               (was Node.cs — CAS entities are "agents", not "nodes")
│   │   ├── AgentComponent.cs      (was CollektiveNodeComponent.cs — follows Agent rename, drops redundant prefix)
│   │   ├── AgentScheduler.cs      (was NodeScheduler.cs — abstract MB, follows Agent rename)
│   │   ├── Neighborhood           (topology is a distinct sub-concern, deserves its own folder)
│   │   │   ├── NeighborhoodComponent.cs    (was NeighboringComponent — "Neighboring" is an adjective, not a noun)
│   │   │   └── NeighborhoodVisualizer.cs   (moved here from root of old folder, belongs with topology)
│   ├── Abstractions               (explicit home for all interfaces, currently scattered)
│   │   ├── IActuator.cs           (keep name)
│   │   ├── ISensor.cs             (keep name)
│   │   └── IScheduler.cs          (was INodeScheduler — "Node" prefix is redundant in this context)
│   ├── Attributes
│   │   └── ReadOnlyAttribute.cs   (keep)
│   ├── Configuration              (was Globals — "Globals" is an implementation detail, not a concept)
│   │   └── SimulationSettings.cs  (was GlobalData.cs — "GlobalData" is extremely vague)
│   ├── Utilities                  (was Utils — minor, but full word is more consistent)
│   │   └── SingletonBehaviour.cs  (keep)
│   ├── Generated
│   │   ├── Shared.cs              (keep)
│   │   └── UserDefinedSchema.cs   (keep)
│   ├── Plugins
│   │   └── libcollektive_backend.so
│   ├── Examples                   (was Example — plural since it's a collection)
│   │   ├── Scheduling
│   │   │   └── FixedTimeScheduler.cs
│   │   ├── Sensors
│   │   │   ├── PositionSensor.cs
│   │   │   └── SourceSensor.cs
│   │   ├── Actuators
│   │   │   └── MotionActuator.cs
│   │   ├── Neighborhood
│   │   │   └── ProximityNeighborhood.cs    (was ProximityNeighboring — same adjective problem)
│   │   └── Visualization
│   │       └── LineNeighborhoodVisualizer.cs
│   └── unibo.collektive.unity.asmdef
```
```
