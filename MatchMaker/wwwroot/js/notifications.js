"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveVoteStarted", function (maps, user) {


    var li = $("#activeVoteItemTemplate").clone();
    $("#activeVotes").append(li);

    if (maps.startingUser === currentUserName) {
        var deleteButton = li.find(".deleteVoteButton");
        deleteButton.on("click", function () { sendDeleteVote(maps.id); });
        deleteButton.show();

    }

    li.attr("id","voteel_" + maps.id);
    li.on("click", function () { showVote(maps.id); });
    UpdateVoteValues(maps);
    li.show();

});

function sendDeleteVote(mapsid) {
    connection.invoke("DeleteVote", mapsid).catch(function (err) {
        return console.error(err.toString());
    });
}

function updateVoteTitle(maps, user) {
    var msg = user + " hat einen Vote gestartet (" + maps.playersVoted.length + ")";
    console.log(msg);
    var li = $("#voteel_" + maps.id);
    li.find(".text").text(msg);
}

function showVote(mapsid) {
    var li = $("#voteel_" + mapsid);
    li.off("click");
    var container = $("#" + mapsid);
    li.on("click", function () { hideVote(li, container, mapsid); });
    container.show();
}

function hideVote(li, container, mapsid) {
    container.hide();
    $(li).off("click");
    $(li).on("click", function () { showVote(mapsid); });
}

function callVote(voteId, mapId) {
    $("#" + voteId).find(".voteButton").attr("disabled", true);
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
    li.css("background-color", "#ffcc00");
    window.setTimeout(function () {
        li.css("background-color", "#d3ffd1");
    }, 100);
    UpdateVoteValues(maps);
});

connection.on("VoteDeleted", function (mapsid) {
    var li = $("#voteel_" + mapsid);
    li.remove();
    var container = $("#" + mapsid);
    container.remove();
});

function UpdateVoteValues(maps) {

    var container = $("#" + maps.id);
    if (container.length === 0) {
        LoadVote(maps);
    }
    var len = maps.playersVoted.length;
    updateVoteTitle(maps, maps.startingUser);

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

function LoadVote(maps) {
   
    var container = $("#" + maps.id);
    if (container.length !== 0) {
        return;
    }
    var li = $("#voteel_" + maps.id);

    container = $(document.createElement("div"));
    container.addClass("row");
    container.attr("id", maps.id);
    li.after(container);
    container.hide();
    $(maps.maps).each(function (i, map) {
        var div = $("#VotingItemTemplate").clone();
        container.append(div);


        div.attr("id", map.id);
        div.find('.voteButton').on("click", function () { callVote(maps.id, map.id) });
        div.find(".mapName").text(map.name);

        div.show();

    });
}