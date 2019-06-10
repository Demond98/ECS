## Определения
Component - компонент

Entity - сущность, состоит из компонентов

System - функция, которая обрабатывает компоненты

типа так:
* Entity
  * Component1
  * Component2
  * Component3


## Создаём компонент __**circle**__
```c#
public class PCircle : CircleShape, IComponent
{
  public PCircle(Vector2f position, float radius, Color color) : base(radius)
  {
    Position = position;
    FillColor = color;
  }
}
```
Интерфейс IComponent обязателен

## Создаём *систему* которая будет его двигать
```c#
class SCircleMoveLeft : ISystem
{
  public void Invoke(IEnumerable<Entity> entities) //передаём все сущности
  {
    foreach (var entity in entities
      .HasFlags(EntityFlags.NeedMoveLeft) //если есть флаг NeedMoveLeft, то двигаем
      .HasComponent<PCircle>()) // если есть нужный компонент

      entity
      .Get<PCircle>() //получаем компонент PCircle
      .Position += new Vector2f(0.1f, 0);
  }
}
```
Интерфейс ISystem обязателен

## Создаём окружение и entity 
```c#
var environment = new Environment();

var entity = environment.CreateEntity(); // создаём сущность
entity.Add(EntityFlags.NeedMoveLeft); //Добавляем флаг
entity.Add(new PCircle(new Vector2f(50, 50), 10f, Color.White)); //добавляем компонент

environment.Systems.Add(new SCircleMoveLeft()); //добавляем функции в окружение
```

## Где-то в игровом цикле
```c#
while (_gameWindow.IsOpen)
{
  _gameWindow.Clear();
  Environment.InvokeSystems(); //выполняем функции
  _gameWindow.Display();
}
```
