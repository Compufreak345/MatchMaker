"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveVoteStarted", function (maps, user) {
    var usr = user.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = usr + " hat einen Vote gestartet (hier klicken zum Anzeigen)";
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    li.id = "voteel_" + maps.id;

    document.getElementById("activeVotes").appendChild(li);
    $(li).on("click", function () { showVote(maps); });
    $(li).addClass("voteStartedItem");

});

function showVote(maps) {
    var li = $("#voteel_" + maps.id);
    $(maps.maps).each(function (i, map) {
        var div = $("#VotingItemTemplate").clone();
        li.after(div);
        li.off("click");

        div.attr("id", map.id);
        div.find('.voteButton').on("click", function () { callVote(maps.id, map.id) });
        div.find(".mapName").text(map.name);

        div.show();

    });
    UpdateVoteValues(maps);
}

function callVote(voteId, mapId) {

    connection.invoke("Vote", voteId, mapId).catch(function (err) {
        return console.error(err.toString());
    });
}

connection.on("Alert", function (message) {
    alert(message);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var maps = document.getElementById("Maps").value;
    connection.invoke("StartVote", maps).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


connection.on("UpdateVote", function (maps) {
    var li = $("#voteel_" + maps.id);
    /*li.css("background-color", "#ffcc00");
    window.setTimeout(function () {
        li.css("background-color", "#d3ffd1");
    }, 200);*/
    UpdateVoteValues(maps);
});

function UpdateVoteValues(maps) {
    var len = maps.playersVoted.length;
    $(maps.maps).each(function (i, map) {
        var mapel = $("#" + map.id).find(".mapVoteBar");
        if (len === 0)
        {
            mapel.text("");
            return;
        }
        var percentage = Math.round(map.voteCount / len * 100);


        mapel.css("width", percentage + "%");
        if (percentage > 0) {
            mapel.text(map.voteCount + " : " + percentage + "%");
        } else {
            mapel.text(map.VoteCount);
        }
    });
}