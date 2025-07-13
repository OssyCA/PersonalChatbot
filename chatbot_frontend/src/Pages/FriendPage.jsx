import React, { useEffect, useState } from "react";
import Allfriends from "../TempData/tempfriends.json";
const FriendPage = () => {
  const checkStatus = () => {
    Allfriends.Friends.forEach((friend) => {
      if (friend.status === "online") {
        console.log(friend);
      }
    });
  };
  useEffect(() => {
    checkStatus();
  }, []);
  return (
    <>
      <h1>My Friends</h1>
      <div className="friendsContainer">
        <div>
          {Allfriends.Friends.map((friend, index) => (
            <div key={index}>
              {friend.name} {friend.status}
            </div>
          ))}
        </div>
      </div>
    </>
  );
};

export default FriendPage;
