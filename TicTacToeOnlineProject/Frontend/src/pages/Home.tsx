import { useState } from "react";
import { useSignalR } from "../context/SignalRContext";
import { useNavigate } from "react-router-dom";
import LoadingContainer from "../components/LoadingContainer";
import LobbyContainer from "../components/LobbyContainer";
import ConnectionFailed from "../components/ConnectionFailed";

const IS_DEV_MODE = true;

export type GameStoreReturnStatus = 
  | "CreateBoardSuccess"
  | "GetBoardSuccess"
  | "GameFull"
  | "ErrorCreatingBoard"
  | "BoardDoesNotExist"
  | "RemoveBoardSuccess"
  | "ErrorRemovingBoard"
  | "BoardNullException";
export type JoinGameReturnStatus = 
  | "GameJoinSuccess"
  | "GameFull"
  | "BoardNotFound"
  | "BoardNullException"
export type AssignMarkStatus = 
  | "Success"
  | "InvalidMark"
  | "AlreadyAssigned"
  | "BoardNotFound"
  | "BoardNullException"

export interface BoardDto {
  boardId: string;
  grid: string[];
  currentTurn: string;
}

export interface CreateGameResponse {
  success: boolean;
  status: GameStoreReturnStatus
  boardDto: BoardDto | null;
}

export interface JoinGameResponse {
  success: boolean;
  status: JoinGameReturnStatus;
  boardDto: BoardDto | null;
}

export interface AssignMarkResponse {
  success: boolean
  status: AssignMarkResponse
  mark: string | null
}

type DevViewMode = "auto" | "connecting" | "failed" | "action-loading" | "lobby";

export default function Home() {
  const navigate = useNavigate();
  const { connection, isConnected, isInitialLoading, reconnect } = useSignalR();
  const [tab, setTab] = useState<"create" | "join">("create");
  const [isLoading, setIsLoading] = useState(false);
  const [isRetrying, setIsRetrying] = useState(false);
  const [devMode, setDevMode] = useState<DevViewMode>("auto");
  const [error, setError] = useState("")

  const handleCreateRoom = async () => {
    setIsLoading(true);

    try {
      const createGameResponse = await connection?.invoke<CreateGameResponse>("CreateGame");
      if (createGameResponse == null || !createGameResponse.success) {
        setError("Failed to create game")
        console.error("Failed to create game:", createGameResponse?.status);
        return
      }

      navigate(`/room/${createGameResponse.boardDto?.boardId}`)
    } catch (err) {
      console.error("Error invoking CreateGame:", err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleJoinRoom = async (roomCode: string) => {
    const code = roomCode.trim().toUpperCase();
    if (!code) return;
    navigate(`/room/${code}`)
  };

  const handleReconnect = async () => {
    setIsRetrying(true);

    try {
      await reconnect();
    } catch (err) {
      console.error("Manual reconnect failed:", err);
    } finally {
      setIsRetrying(false);
    }
  };

  const renderCurrentView = () => {
    if (IS_DEV_MODE && devMode !== "auto") {
      if (devMode === "connecting") return <LoadingContainer text="Connecting to server" styleSelected="bg-[#FACC15]" />;
      if (devMode === "failed") {
        return (
          <div className="w-full max-w-sm mx-auto flex flex-col items-center">
            <ConnectionFailed text="Failed to connect to the server" functionCallback={handleReconnect} />
          </div>
        );
      }
      if (devMode === "action-loading") return <LoadingContainer text="Loading" styleSelected="bg-[#FACC15]" />;
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

    if (isInitialLoading || isRetrying) {
      return <LoadingContainer text="Connecting to server" styleSelected="bg-[#FACC15]" />;
    }

    if (!isConnected) {
      return (
        <div className="w-full max-w-sm mx-auto flex flex-col items-center">
          <ConnectionFailed text="Failed to connect to the server" functionCallback={handleReconnect} />
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
      {IS_DEV_MODE && (
        <div className="fixed top-3 left-3 z-50 flex flex-wrap gap-1.5 rounded-lg border border-[#1E293B] bg-white p-1.5 shadow-md">
          {(["auto", "connecting", "failed", "action-loading", "lobby"] as DevViewMode[]).map((mode) => (
            <button
              key={mode}
              type="button"
              onClick={() => setDevMode(mode)}
              className={`rounded px-2 py-1 text-xs font-bold transition-all ${
                devMode === mode ? "bg-[#1E293B] text-white" : "bg-slate-100 text-slate-700 hover:bg-slate-200"
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