import Register from "./Pages/Register";
import Login from "./Pages/Login";
import StartScreen from "./Pages/StartScreen";
import ChatBot from "./Pages/ChatBot";
import ProtectedRoute from "./Comp/ProtectedRoute";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

function App() {
  return (
    <>
      <Router>
        <Routes>
          <Route path="/" element={<StartScreen />} />
          <Route path="/register" element={<Register />} />
          <Route path="/Login" element={<Login />} />

          <Route element={<ProtectedRoute />}>
            <Route path="/chatbot" element={<ChatBot />} />
          </Route>
        </Routes>
      </Router>
    </>
  );
}

export default App;
