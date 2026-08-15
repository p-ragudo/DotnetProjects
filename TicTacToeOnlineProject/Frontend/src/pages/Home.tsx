import { useState } from "react";
import { useSignalR } from "../context/SignalRContext";
import LoadingContainer from "../components/LoadingContainer";
import LobbyContainer from "../components/LobbyContainer";
import ConnectionFailed from "../components/ConnectionFailed";

// ==========================================
// ⚙️ TOGGLE THIS TO SWITCH DEV / PROD MODE
// ==========================================
const IS_DEV_MODE = true; // Set to `false` for production

export type PlayerMark = 'X' | 'O' | '';

export interface BoardDto {
  boardId: string;
  grid: string[];
  currentTurn: PlayerMark;
}

export interface CreateGameResponse {
  success: boolean;
  status: string;
  boardDto: BoardDto | null;
}

type DevViewMode = "auto" | "connecting" | "failed" | "action-loading" | "lobby";

export default function Home() {
  const { connection, isConnected, isInitialLoading, reconnect } = useSignalR();
  const [tab, setTab] = useState<"create" | "join">("create");
  const [roomCode, setRoomCode] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isRetrying, setIsRetrying] = useState(false);

  // Active view override for dev mode
  const [devMode, setDevMode] = useState<DevViewMode>("auto");

  const handleCreateRoom = async () => {
    setIsLoading(true);
    try {
      const response = await connection?.invoke<CreateGameResponse>("CreateGame");
      if (response?.success) {
        console.log("Game created with ID: ", response.boardDto?.boardId);
      } else {
        console.error("Failed to create game: ", response?.status);
      }
    } catch (err) {
      console.error('Error invoking CreateGame at Home.tsx:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleJoinRoom = async () => {
    setIsLoading(true);
    try {
      // Call JoinGame invoke here
    } catch (err) {
      console.error('Error invoking JoinGame at Home.tsx:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleReconnect = async () => {
    setIsRetrying(true);
    try {
      await reconnect();
    } catch (err) {
      console.error("Manual reconnect failed: ", err);
    } finally {
      setIsRetrying(false);
    }
  };

  const renderCurrentView = () => {
    // 1. Dev Mode overrides (only active when IS_DEV_MODE is true and not set to "auto")
    if (IS_DEV_MODE && devMode !== "auto") {
      if (devMode === "connecting") {
        return <LoadingContainer text="Connecting to server" styleSelected="bg-[#FACC15]" />;
      }
      if (devMode === "failed") {
        return (
          <div className="w-full max-w-sm mx-auto flex flex-col items-center">
            <ConnectionFailed 
              text="Failed to connect to the server"
              functionCallback={handleReconnect}
            />
          </div>
        );
      }
      if (devMode === "action-loading") {
        return <LoadingContainer text="Loading" styleSelected="bg-[#FACC15]" />;
      }
      if (devMode === "lobby") {
        return (
          <LobbyContainer 
            setTab={setTab}
            selectedTab={tab}
            handleCreateRoom={handleCreateRoom}
            handleJoinRoom={handleJoinRoom}
          />
        );
      }
    }

    // 2. Real Production / SignalR Logic
    if (isInitialLoading || isRetrying) {
      return <LoadingContainer text="Connecting to server" styleSelected="bg-[#FACC15]" />;
    }

    if (!isConnected) {
      return (
        <div className="w-full max-w-sm mx-auto flex flex-col items-center">
          <ConnectionFailed 
            text="Failed to connect to the server"
            functionCallback={handleReconnect}
          />
        </div>
      );
    }

    if (isLoading) {
      return <LoadingContainer text="Loading" styleSelected="bg-[#FACC15]" />;
    }

    return (
      <LobbyContainer 
        setTab={setTab}
        selectedTab={tab}
        handleCreateRoom={handleCreateRoom}
        handleJoinRoom={handleJoinRoom}
      />
    );
  };

  return (
    <>
      {/* Dev toolbar only renders when IS_DEV_MODE is true */}
      {IS_DEV_MODE && (
        <div className="fixed top-3 left-3 z-50 flex flex-wrap gap-1.5 rounded-lg border border-[#1E293B] bg-white p-1.5 shadow-md">
          {(["auto", "connecting", "failed", "action-loading", "lobby"] as DevViewMode[]).map((mode) => (
            <button
              key={mode}
              type="button"
              onClick={() => setDevMode(mode)}
              className={`rounded px-2 py-1 text-xs font-bold transition-all ${
                devMode === mode
                  ? "bg-[#1E293B] text-white"
                  : "bg-slate-100 text-slate-700 hover:bg-slate-200"
              }`}
            >
              {mode}
            </button>
          ))}
        </div>
      )}

      {renderCurrentView()}
    </>
  );
}