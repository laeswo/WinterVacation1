using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team {
    Red,
    Blue
}

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem instance;

    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static PlayerScript GetParticularPlayer(Team team) {
        if (team == Team.Red) {
            return PlayerScript.players.Find(v => v.team == "red");
        } else if (team == Team.Blue) {
            return PlayerScript.players.Find(v => v.team == "blue");
        }

        return null;
    }

    public static List<PlayerScript> GetAllPlayers() {
        return PlayerScript.players;
    }

    public static List<PlayerScript> GetPlayersOfCenter(Vector2 centerPos, float radius) {
        Collider2D[] colls = Physics2D.OverlapCircleAll(new Vector3(centerPos.x, centerPos.y), radius);

        List<PlayerScript> players = new();

        foreach (Collider2D col in colls) {
            var ps = col.transform.GetComponent<PlayerScript>();

            if (ps != null) players.Add(ps);
        }

        return players;
    }

    public static PlayerScript GetTargetPlayer(PlayerScript origin, float radius, Vector3? centerPos = null) {
        Vector3 pos = origin.transform.position;
        if (centerPos.HasValue) {
            pos = centerPos.Value;
        }

        Collider2D[] colls = Physics2D.OverlapCircleAll(new Vector3(pos.x, pos.x), radius);

        List<PlayerScript> players = new();

        foreach (Collider2D col in colls) {
            var ps = col.transform.GetComponent<PlayerScript>();

            if (ps != null) {
                return ps;
            }
        }

        return null;
    }
}
