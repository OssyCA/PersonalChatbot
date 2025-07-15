import React, { useEffect, useState } from "react";
import Allfriends from "../TempData/tempfriends.json";
import { getStatusColor } from "../Utils/ColorStatus";
const FriendPage = () => {
  return (
    <>
      <h1>My Friends</h1>
      <div className="friendsContainer">
        {Allfriends.Friends.map((friend, i) => (
          <div key={i} className="friendDisplay">
            {friend.name}
            <span
              style={{
                display: "inline-block",
                width: 10,
                height: 10,
                borderRadius: "50%",
                marginLeft: 8,
                backgroundColor: getStatusColor(friend.status),
              }}
            />
          </div>
        ))}
      </div>
    </>
  );
};

export default FriendPage;
