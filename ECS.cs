using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;

namespace Genetic.ECS
{
	[Flags]
	public enum EntityFlags
	{
		NeedMoveLeft = 1
	}

	public interface ISystem
	{
		void Invoke(IEnumerable<Entity> entities);
	}

	public interface IDrawSystem
	{
		void Draw(RenderWindow window, IEnumerable<Entity> entities);
	}

	public interface IComponent
	{

	}

	internal static class TypeOf<T> where T : class, IComponent
	{
		public static readonly RuntimeTypeHandle handle = typeof(T).TypeHandle;
	}

	internal sealed class ComponentsCollection
	{
		private Dictionary<RuntimeTypeHandle, Dictionary<Entity, IComponent>> _componentsArray
			= new Dictionary<RuntimeTypeHandle, Dictionary<Entity, IComponent>>();

		public void Add<T>(Entity entity, T component)
			where T : class, IComponent
		{
			if (!_componentsArray.ContainsKey(TypeOf<T>.handle))
				_componentsArray.Add(TypeOf<T>.handle, new Dictionary<Entity, IComponent>());

			if (_componentsArray[TypeOf<T>.handle].ContainsKey(entity))
				_componentsArray[TypeOf<T>.handle][entity] = component;

			else
				_componentsArray[TypeOf<T>.handle].Add(entity, component);
		}

		public Dictionary<Entity, IComponent> Get<T>() where T : class, IComponent
			=> _componentsArray[TypeOf<T>.handle];

		public T Get<T>(Entity entity) where T : class, IComponent
			=> _componentsArray[TypeOf<T>.handle][entity] as T;

		public void Set<T>(Entity entity, T component) where T : class, IComponent
			=> _componentsArray[TypeOf<T>.handle][entity] = component;

		public void Remove<T>(Entity entity) where T : class, IComponent
			=> _componentsArray[TypeOf<T>.handle].Remove(entity);

		public void Remove(Entity entity)
		{
			foreach (var type in entity.Components)
				_componentsArray[type].Remove(entity);
		}
	}

	public sealed class Entity
	{
		private EntityFlags _flags;
		private ComponentsCollection _collection;
		private List<RuntimeTypeHandle> _components = new List<RuntimeTypeHandle>();
		public IReadOnlyList<RuntimeTypeHandle> Components => _components.AsReadOnly();

		internal Entity(ComponentsCollection collection, EntityFlags flags)
		{
			_collection = collection;
			_flags = flags;
		}

		public void AddFlag(EntityFlags flag)
			=> _flags |= flag;

		public void RemoveFlag(EntityFlags flag)
			=> _flags ^= flag;

		public bool HasFlag(EntityFlags flag)
			=> _flags.HasFlag(flag);

		public void Add<T>(T component)
			where T : class, IComponent
		{
			_collection.Add(this, component);
			_components.Add(TypeOf<T>.handle);
		}

		public bool Has<T>() where T : class, IComponent
			=> _components.Contains(TypeOf<T>.handle);

		public T Get<T>() where T : class, IComponent
			=> _collection.Get<T>(this);

		public void Set<T>(T component) where T : class, IComponent
			=> _collection.Set(this, component);

		public void Remove<T>() where T : class, IComponent
			=> _collection.Remove<T>(this);

		public void Remove()
			=> _collection.Remove(this);
	}

	public sealed class Environment
	{
		public LinkedList<Entity> Entities { get; set; } = new LinkedList<Entity>();
		private ComponentsCollection _componentsCollection = new ComponentsCollection();

		public List<ISystem> Systems { get; set; } = new List<ISystem>();
		public List<IDrawSystem> DrawSystems { get; set; } = new List<IDrawSystem>();

		public void InvokeSystems() => Systems.ForEach(a => a.Invoke(Entities));
		public void InvokeDrawSystems(RenderWindow window) => DrawSystems.ForEach(a => a.Draw(window, Entities));

		public Entity CreateEntity(EntityFlags flags)
			=> Entities.AddLast(new Entity(_componentsCollection, flags)).Value;
	}

	public static class EntityExtension
	{
		public static IEnumerable<Entity> HasFlags(this IEnumerable<Entity> entities, EntityFlags flags)
			=> entities.Where(a => a.HasFlag(flags));

		public static IEnumerable<Entity> HasComponent<T>(this IEnumerable<Entity> entities) where T : class, IComponent
			=> entities.Where(a => a.Has<T>());
	}
}

/*
public class Entity
{
	private Dictionary<RuntimeTypeHandle, IProperety> keyValues;

	public IProperety Get<T>()
	{
		return keyValues[typeof(T).TypeHandle];
	}

	private List<IProperety> _propereties = new List<IProperety>();



	public void AddPropereties(params IProperety[] propereties)
		=> _propereties.AddRange(propereties);

	public bool HavePropereties<T>()
		where T : class, IProperety
		=> _propereties.Exists(a => a is T);

	public bool HavePropereties<T0, T1>()
		where T0 : class, IProperety
		where T1 : class, IProperety
		=> _propereties.FindAll(a => a is T0 || a is T1).Count >= 2;

	public bool HavePropereties<T0, T1, T2>()
		where T0 : class, IProperety
		where T1 : class, IProperety
		where T2 : class, IProperety
		=> _propereties.FindAll(a => a is T0 || a is T1 || a is T2).Count >= 3;

	public T GetPropereties<T>()
		where T : class, IProperety
		=> _propereties.First(a => a is T) as T;

	public (T0, T1) GetPropereties<T0, T1>()
		where T0 : class, IProperety
		where T1 : class, IProperety
		=> (
			_propereties.First(a => a is T0) as T0,
			_propereties.First(a => a is T1) as T1
		);

	public (T0, T1, T2) GetPropereties<T0, T1, T2>()
		where T0 : class, IProperety
		where T1 : class, IProperety
		where T2 : class, IProperety
		=> (
			_propereties.First(a => a is T0) as T0,
			_propereties.First(a => a is T1) as T1,
			_propereties.First(a => a is T2) as T2
		);
}*/
