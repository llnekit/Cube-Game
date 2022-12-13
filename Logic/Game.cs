﻿using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml.Linq;

namespace Logic
{
    internal class Map
    {
        private List<List<int>> _map;
        private Vector3[] _vecMap;

        private readonly string _defaultPath = "Map/";
        private readonly string _defaultMap = "default.txt";
        public Vector3 _startPos;
        public Vector3 _endPos;
        private Side _startSide = Side.LEFT;
        public Side StartSide => _startSide;
        private Side _endSide = Side.DOWN;
        public Side EndSide => _endSide;
        public Map()
        {
            this.Update(_defaultMap);
        }

        public void Update (string path)
        {
            UpdateIntMap(path);
            UpdateVecMap();
        }

        private void UpdateIntMap(string Path)
        {
            var newMap = new List<List<int>>();
            try
            {
                using (StreamReader reader = new StreamReader(_defaultPath + Path))
                {
                    var startSideFromFile = reader.ReadLine();
                    if (startSideFromFile != null && startSideFromFile.Length == 1) _startSide = (Side)int.Parse(startSideFromFile);

                    var endSideFromFile = reader.ReadLine();
                    if (endSideFromFile != null && endSideFromFile.Length == 1) _endSide = (Side)int.Parse(endSideFromFile);

                    string text = reader.ReadToEnd();

                    var rows = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var row in rows)
                    {
                        var rowElements = new List<int>();
                        var cols = row.Split('\t');
                        if (cols.Count() == 0) cols = row.Split(' ');
                        foreach (var col in cols)
                        {
                            rowElements.Add(Int32.Parse(col));
                        }
                        newMap.Add(rowElements);
                    }
                }

                _map = newMap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении карты {ex.Message}");
            }
            
        }

        internal List<List<int>> IntMap => _map;

        public int getMapValue(int x, int y)
        {
            return _map[_map.Count - y - 1][x];
        }

        private void UpdateVecMap()
        {
            var list = new List<Vector3>();

            bool hasStart = false;
            bool hasEnd = false;

            try
            {
                for (int i = 0; i < _map.Count; i++)
                {
                    for (int j = 0; j < _map[i].Count; j++)
                    {
                        if (_map[i][j] == 1)
                        {
                            list.Add(new Vector3((float)j, 0.0f, (float)i));
                        }
                        if (_map[i][j] == 2 && !hasStart)
                        {
                            hasStart = true;
                            _startPos = new Vector3((float)j, 1.0f, (float)i);
                            list.Add(new Vector3((float)j, 0.0f, (float)i));
                        }
                        if (_map[i][j] == 3 && !hasEnd)
                        {
                            hasEnd = true;
                            //_endPos3D = new Vector3((float)j, 0.0f, (float)i);
                            _endPos = new Vector3((float)j, 0.0f, (float)i);
                            list.Add(_endPos);

                        }
                    }
                }

                if (!hasStart) throw new Exception("Не найден старт");
                if (!hasEnd) throw new Exception("Не найден финиш");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении карты: {ex.Message}");
            }


            _vecMap = list.ToArray();
        }

        public Vector3[] VectorMap => _vecMap;

        /*public Vector3[] GetMap()
        {
            var list = new List<Vector3>();

            bool hasStart = false;
            bool hasEnd = false;

            try
            {
                for (int i = 0; i < _map.Count; i++)
                {
                    for (int j = 0; j < _map[i].Count; j++)
                    {
                        if (_map[i][j] == 1)
                        {
                            list.Add(new Vector3((float)j, 0.0f, (float)i));
                        }
                        if (_map[i][j] == 2 && !hasStart)
                        {
                            hasStart = true;
                            _startPos = new Vector3((float)j, 1.0f, (float)i);
                            list.Add(new Vector3((float)j, 0.0f, (float)i));
                        }
                        if (_map[i][j] == 3 && !hasEnd)
                        {
                            hasEnd = true;
                            //_endPos3D = new Vector3((float)j, 0.0f, (float)i);
                            _endPos = new Vector3((float)j, 0.0f, (float)i);
                            list.Add(_endPos);

                        }
                    }
                }

                if (!hasStart) throw new Exception("Не найден старт");
                if (!hasEnd) throw new Exception("Не найден финиш");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении карты: {ex.Message}");
            }

            
            return list.ToArray();
        }*/

    }

    public enum Direction { LEFT, RIGHT, UP, DOWN }

    public enum Side { FORWARD, BACK, LEFT, RIGHT, DOWN, UP}

    public class State
    {
        public int X, Y;
        public Side Side;

        public State() { }

        public State (State state) { X = state.X; Y = state.Y; Side = state.Side; }

        public override string ToString()
        {
            return $"[{X};{Y}]: {Side}";
        }
        public static bool operator ==(State a, State b) => a?.X == b?.X && a?.Y == b?.Y && a?.Side == b?.Side;

        public static bool operator !=(State a, State b) => !(a?.X == b?.X && a?.Y == b?.Y && a?.Side == b?.Side);


    }

    internal class Player
    {
        /// <summary>
        /// Координаты игрового кубика
        /// </summary>
        public Vector3 _position;
        public Side _playerSide;
        public int _mapSize;

        public void Move(State state)
        {
            _position.X = state.X;
            _position.Z = _mapSize - state.Y - 1;
            _playerSide = state.Side;
        }
        
    }

    public class Node
    {
        public State value;
        public Node parent;
        public Direction dirToThisState;
    }

    internal class AI
    {
        
        private readonly Game _game;

        private bool _isWorking = false;

        public bool IsWorking => _isWorking;

        private State _start;

        public State StartState => _start;

        private State _finishState;

        public State FinishState => _finishState;

        private State _currentState;

        public State CurrentState { get { return _currentState; } set { _currentState = value; } }
            
        private Queue<Node> O = new Queue<Node>();

        private Dictionary<string, State> C = new Dictionary<string, State>();

        private bool isFinish(State state) => state == _finishState;

        private double timer = 0;

        public double WorkingTime => timer;

        public AI(Game game) => _game = game;

        private List<Node> _wayToFinish = new List<Node>();

        public List<Node> WayToFinish => _wayToFinish;

        private List<string> _AIInfo = new List<string>();

        public List<string> AIInfo => _AIInfo;

        private int _iterationCounter = 0;

        private int _maxOCount = 0;

        private int _maxOAndCCount = 0;

        public void UpdateStates(State startState, State finishState)
        {
            O.Clear();
            C.Clear();
            _AIInfo.Clear();
            _wayToFinish.Clear();
            _iterationCounter = 0;
            _maxOCount = 0;
            _maxOAndCCount = 0;
            _start = startState;
            _currentState = _start;
            O.Enqueue(new Node { value = startState, parent = null });
            _finishState = finishState;
        }

        public void Start()
        {
            timer = 0;
            _isWorking = true;
        }

        public bool Work(double time)
        {
            timer += time;
            _iterationCounter++;

            if (_maxOCount < O.Count)
                _maxOCount = O.Count;

            if (_maxOAndCCount < O.Count + C.Count)
                _maxOAndCCount = O.Count + C.Count;

            var node = O.Dequeue();

            _game.MovePlayer(node.value);

            if (isFinish(CurrentState))
            {
                while (node != null)
                {
                    _wayToFinish.Add(node);
                    node = node.parent;
                }
                
                _wayToFinish.Reverse();
                _wayToFinish.Remove(_wayToFinish.First());

                _AIInfo.Add($"Количество итераций цикла поиска: {_iterationCounter}");
                _AIInfo.Add($"Максимальное количество узлов в списке O: {_maxOCount}");
                _AIInfo.Add($"Количество узлов в списке O в конце поиска: {O.Count}");
                _AIInfo.Add($"Максимальное количество хранимых в памяти узлов: {_maxOAndCCount}");
                

                _isWorking = false;
                return true;
            }

            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                var possibleState = _game.CanMove(dir);
                if (possibleState != null && !C.ContainsKey(possibleState.ToString()))
                {
                    O.Enqueue(new Node { value = possibleState, parent = node, dirToThisState = dir });
                }
            }
            C[CurrentState.ToString()] = CurrentState;

            if (O.Count == 0)
            {
                _wayToFinish.Clear();
                _AIInfo.Add("Решений нет");
                _isWorking = false;
                return true;
            }

            return false;
        }
    }

    public class Game
    {
        private Map _gameMap;
        private Player _player;
        private AI _AI;

        public Game()
        {
            _gameMap = new Map();
            _player = new Player();
            _AI = new AI(this);
            _player._mapSize = _gameMap.IntMap.Count;
            _player._position = _gameMap._startPos;
            _player._position.Y += 0.1f;
            _player._playerSide = _gameMap.StartSide;
            UpdateAIStates();
        }

        public void UpdateAIStates()
        {
            var startState = new State
            {
                X = (int)_player._position.X,
                Y = _player._mapSize - (int)_player._position.Z - 1,
                Side = _player._playerSide
            };

            var endState = new State
            {
                X = (int)_gameMap._endPos.X,
                Y = _player._mapSize - (int)_gameMap._endPos.Z - 1,
                Side = _gameMap.EndSide
            };

            _AI.UpdateStates(startState, endState);
        }

        public Vector3[] UpdateMap(string path)
        {
            _gameMap.Update(path);
            _player._mapSize = _gameMap.IntMap.Count;
            _player._position = _gameMap._startPos;
            _player._position.Y += 0.1f;
            _player._playerSide = _gameMap.StartSide;
            UpdateAIStates();
            return _gameMap.VectorMap;
        }

        public Side CurrentSide => _player._playerSide;

        public Vector3[] Map => _gameMap.VectorMap;

        public State CurrentState => _AI.CurrentState;

        public State FinishState => _AI.FinishState;

        public State StartState => _AI.StartState;

        public bool AIIsWorking => _AI.IsWorking;

        /// <summary>
        /// Получить координаты финиша в 3D пространстве
        /// </summary>
        public Vector3 EndPos => _gameMap._endPos;

        /// <summary>
        /// Получить координты игрового кубика в 3D пространстве
        /// </summary>
        public Vector3 PlayerPos => _player._position;

        public State CanMove(Direction dir)
        {
            var state = new State(_AI.CurrentState);

            try
            {
                switch(dir)
                {
                    case Direction.LEFT:
                        switch (state.Side)
                        {
                            case Side.LEFT:     state.Side = Side.DOWN; break;
                            case Side.RIGHT:    state.Side = Side.UP; break;
                            case Side.FORWARD:  state.Side = Side.FORWARD; break;
                            case Side.BACK:     state.Side = Side.BACK; break;
                            case Side.UP:       state.Side = Side.LEFT; break;
                            case Side.DOWN:     state.Side = Side.RIGHT; break;
                        }
                        state.X--;
                        break;
                    case Direction.RIGHT:
                        switch (state.Side)
                        {
                            case Side.LEFT:     state.Side = Side.UP; break;
                            case Side.RIGHT:    state.Side = Side.DOWN; break;
                            case Side.FORWARD:  state.Side = Side.FORWARD; break;
                            case Side.BACK:     state.Side = Side.BACK; break;
                            case Side.UP:       state.Side = Side.RIGHT; break;
                            case Side.DOWN:     state.Side = Side.LEFT; break;
                        }
                        state.X++; 
                        break;
                    case Direction.UP:
                        switch (state.Side)
                        {
                            case Side.LEFT:     state.Side = Side.LEFT; break;
                            case Side.RIGHT:    state.Side = Side.RIGHT; break;
                            case Side.FORWARD:  state.Side = Side.DOWN; break;
                            case Side.BACK:     state.Side = Side.UP; break;
                            case Side.UP:       state.Side = Side.FORWARD; break;
                            case Side.DOWN:     state.Side = Side.BACK; break;
                        }
                        state.Y++; 
                        break;
                    case Direction.DOWN:
                        switch (state.Side)
                        {
                            case Side.LEFT:     state.Side = Side.LEFT; break;
                            case Side.RIGHT:    state.Side = Side.RIGHT; break;
                            case Side.FORWARD:  state.Side = Side.UP; break;
                            case Side.BACK:     state.Side = Side.DOWN; break;
                            case Side.UP:       state.Side = Side.BACK; break;
                            case Side.DOWN:     state.Side = Side.FORWARD; break;
                        }
                        state.Y--; 
                        break;
                }

                /*foreach (var row in map)
                {
                    Console.WriteLine();
                    foreach (var col in row)
                        Console.Write(col.ToString() + " ");
                }
                Console.WriteLine();*/
                //return state;
                var mapValue = _gameMap.getMapValue(state.X, state.Y);
                //Console.WriteLine($"Желаемые координаты _x = {state.X} _y = {state.Y} map_value = {mapValue}");
                if (mapValue != 0) return state;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Выход за пределы");
            }
            return null;
        }

        public int MovePlayer(State state)
        {
            if (state != null)
            {
                _player.Move(state);

                _AI.CurrentState = state;
            }

            return (int)_player._playerSide;
        }

        public int MovePlayer(Direction dir) 
        {
            var newState = CanMove(dir);

            return MovePlayer(newState);
        }

        public void StartAI()
        {
            UpdateAIStates();
            _AI.Start();
        }

        public List<Node> WayToFinish => _AI.WayToFinish;

        public List<string> AIInfo => _AI.AIInfo;

        public bool GetAINextStep(double time) => _AI.Work(time);

        public double AIWorkingTime => _AI.WorkingTime;

        public List<State> GetStartStates (int d)
        {
            if (d < 2) return null;
            _AI.CurrentState = this.FinishState;
            var oldStates = new List<State> ();
            oldStates.Add(this.FinishState);
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                var possibleState = this.CanMove(dir);
                if (possibleState != null)
                {
                    oldStates.Add(possibleState);
                }
            }
            var prevStates = new List<State>();
            foreach (var state in oldStates)
                prevStates.Add(state);
            for (int i = 2; i <= d; i++)
            {
                var tmp = new List<State>();
                foreach (var prevState in prevStates)
                {
                    _AI.CurrentState = prevState;
                    foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                    {
                        var possibleState = this.CanMove(dir);
                        if (possibleState != null && !oldStates.Any(x => x == possibleState))
                        {
                            tmp.Add(possibleState);
                            oldStates.Add(possibleState);
                        }
                    }

                }
                prevStates = tmp;
            }
            return prevStates;
        }

    }
    
    

}