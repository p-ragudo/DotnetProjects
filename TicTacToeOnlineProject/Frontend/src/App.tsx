import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { GameLayout } from "./components/GameLayout";
import Home from "./pages/Home";
import GameBoard from "./pages/GameBoard";

export default function App() {
  return (
    <BrowserRouter>
      <GameLayout>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/room/:roomCode" element={<GameBoard />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </GameLayout>
    </BrowserRouter>
  );
}