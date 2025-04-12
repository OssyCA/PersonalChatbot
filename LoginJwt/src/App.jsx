import Register from "./Pages/Register";
import Login from "./Pages/Login";
import StartScreen from "./Pages/StartScreen";
import ChatBot from "./Pages/ChatBot";
import ProtectedRoute from "./Comp/ProtectedRoute";
import UserDashboard from "./Pages/UserDashboard";
import Navbar from "./Comp/Navbar";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

function App() {
  return (
    <>
      <Router>
        <Navbar />
        <main>
          <Routes>
            <Route path="/" element={<StartScreen />} />
            <Route path="/register" element={<Register />} />
            <Route path="/Login" element={<Login />} />
            <Route path="/start-screen" element={<StartScreen />} />
            {/* Protected Routes*/}
            <Route element={<ProtectedRoute />}>
              <Route path="/chatbot" element={<ChatBot />} />
              <Route path="/user-dashboard" element={<UserDashboard />} />
            </Route>
          </Routes>
        </main>
      </Router>
    </>
  );
}

export default App;
