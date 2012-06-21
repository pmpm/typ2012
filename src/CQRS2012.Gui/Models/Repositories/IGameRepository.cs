using System;
using System.Collections.Generic;

namespace CQRS2012.Gui.Models.Repositories
{
    //TODO: zrefaktoryzowac, prawdopodbnie bedzie trzeba rozbic na interfejs do Game i oddzielny do Result
    //TODO: skoro repozytorim nazywa sie GameRepository to po co jest uzywana nazwa Game w kazdej metodzie
    public interface IGameRepository
    {
        Game GetGame(Guid id);
        IEnumerable<Game> GetAllGames();
        IEnumerable<Game> GetFinishedGames();
        void SaveGame(Game game);
        void UpdateGame(Game game);
        void FinishGame(Game game);
        void DeleteGame(Game game);

        void UpdateResult(Result result);

        IEnumerable<Team> GetAllTeams();
        Team GetTeam(Guid id);
        void SaveTeam(Team team);
    }
}