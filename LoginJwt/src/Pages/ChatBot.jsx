import React, { useState, useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { authFetch } from "../Utils/AuthUtils"; // Import the authFetch utility

const ChatBot = () => {
  const [messages, setMessages] = useState([]);
  const [input, setInput] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const messagesEndRef = useRef(null);
  const navigate = useNavigate();

  // Automatically scroll to the bottom of the messages
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  // Add a welcome message when component mounts
  useEffect(() => {
    const username = localStorage.getItem("username") || "User";
    setMessages([
      {
        text: `Hello ${username}! How can I help you today?`,
        sender: "bot",
      },
    ]);
  }, []);

  const sendMessage = () => {
    if (input.trim() && !isLoading) {
      const userMessage = input.trim();

      // Add user message to the chat
      setMessages((prevMessages) => [
        ...prevMessages,
        { text: userMessage, sender: "user" },
      ]);

      setInput("");
      setIsLoading(true);

      // Get bot response
      botResponse(userMessage);
    }
  };

  const botResponse = async (userMessage) => {
    try {
      // Use the authFetch utility that handles token refresh
      const response = await authFetch(
        `https://localhost:7289/InputMessage/${encodeURIComponent(
          userMessage
        )}`,
        {
          method: "POST",
        }
      );

      if (!response.ok) {
        throw new Error(
          `Failed to fetch: ${response.status} ${response.statusText}`
        );
      }

      const responseData = await response.json();

      // Add bot message to the chat
      setMessages((prevMessages) => [
        ...prevMessages,
        { text: responseData, sender: "bot" },
      ]);
    } catch (error) {
      console.error("Error:", error);

      // Show error message in chat
      setMessages((prevMessages) => [
        ...prevMessages,
        {
          text: "Sorry, there was an error processing your request. Please try again.",
          sender: "bot",
          isError: true,
        },
      ]);
    } finally {
      setIsLoading(false);
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      sendMessage();
    }
  };

  const handleLogout = async () => {
    try {
      await fetch("https://localhost:7289/logout", {
        method: "POST",
        credentials: "include",
      });
    } catch (error) {
      console.error("Logout error:", error);
    } finally {
      localStorage.clear();
      navigate("/");
    }
  };

  const handleBackToDashboard = () => {
    navigate("/user-dashboard");
  };

  return (
    <div className="chat-container">
      <div className="chat-header">
        <h1>ChatBot</h1>
        <div className="header-buttons">
          <button onClick={handleBackToDashboard} className="back-button">
            Dashboard
          </button>
          <button onClick={handleLogout} className="logout-button">
            Sign out
          </button>
        </div>
      </div>

      <div className="chat-messages">
        {messages.length === 0 ? (
          <div className="empty-chat">
            <p>Start a conversation with the bot!</p>
          </div>
        ) : (
          messages.map((msg, index) => (
            <div
              key={index}
              className={`message ${msg.sender} ${msg.isError ? "error" : ""}`}
            >
              {msg.text}
            </div>
          ))
        )}
        {isLoading && (
          <div className="message bot loading">
            <div className="typing-indicator">
              <span></span>
              <span></span>
              <span></span>
            </div>
          </div>
        )}
        <div ref={messagesEndRef} />
      </div>

      <div className="chat-input-area">
        <textarea
          placeholder="Type a message..."
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyDown={handleKeyDown}
          disabled={isLoading}
          rows={1}
        />
        <button onClick={sendMessage} disabled={isLoading || !input.trim()}>
          Send
        </button>
      </div>
    </div>
  );
};

export default ChatBot;
