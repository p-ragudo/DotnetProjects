import { useState } from "react";
import { useSignalR } from "../context/SignalRContext";
import { useNavigate } from "react-router-dom";
import LoadingContainer from "../components/LoadingContainer";
import LobbyContainer from "../components/LobbyContainer";
import ConnectionFailed from "../components/ConnectionFailed";
import StylizedButton from "../components/StylizedButton";

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
  | "BoardNullException";

export type AssignMarkStatus = 
  | "Success"
  | "InvalidMark"
  | "AlreadyAssigned"
  | "BoardNotFound"
  | "BoardNullException";

export interface BoardDto {
  boardId: string;
  grid: string[];
  currentTurn: string;
  playersPresent: number;
  playerMarks: Record<string, string>;
}

export interface CreateGameResponse {
  success: boolean;
  status: GameStoreReturnStatus;
  boardDto: BoardDto | null;
}

export interface JoinGameResponse {
  success: boolean;
  status: JoinGameReturnStatus;
  boardDto: BoardDto | null;
}

export interface AssignMarkResponse {
  success: boolean;
  status: AssignMarkStatus;
  mark: string | null;
}

type DevViewMode = "auto" | "connecting" | "failed" | "action-loading" | "lobby";

export default function Home() {
  const navigate = useNavigate();
  const { connection, isConnected, isInitialLoading, reconnect } = useSignalR();
  const [tab, setTab] = useState<"create" | "join">("create");
  const [isLoading, setIsLoading] = useState(false);
  const [isRetrying, setIsRetrying] = useState(false);
  const [devMode, setDevMode] = useState<DevViewMode>("auto");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleCreateRoom = async () => {
    setIsLoading(true);

    try {
      const createGameResponse = await connection?.invoke<CreateGameResponse>("CreateGame");
      if (createGameResponse == null || !createGameResponse.success) {
        setErrorMessage("Failed to create game room. Please try again.");
        console.error("Failed to create game:", createGameResponse?.status);
        return;
      }

      navigate(`/room/${createGameResponse.boardDto?.boardId}`);
    } catch (err) {
      setErrorMessage("Network error while creating room.");
      console.error("Error invoking CreateGame:", err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleJoinRoom = async (roomCode: string) => {
    const code = roomCode.trim().toUpperCase();
    if (!code) {
      setErrorMessage("Please enter a room code.");
      return;
    }

    setIsLoading(true);

    try {
      const joinResponse = await connection?.invoke<JoinGameResponse>("JoinGameRoom", code);

      if (joinResponse == null || !joinResponse.success) {
        if (joinResponse?.status === "BoardNotFound" || joinResponse?.status === "BoardNullException") {
          setErrorMessage("Room not found. Make sure the code is correct!");
        } else if (joinResponse?.status === "GameFull") {
          setErrorMessage("This room is already full.");
        } else {
          setErrorMessage("Failed to join room. Please try again.");
        }
        return;
      }

      navigate(`/room/${code}`);
    } catch (err) {
      setErrorMessage("Connection error while joining room.");
      console.error("Error invoking JoinGameRoom:", err);
    } finally {
      setIsLoading(false);
    }
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
      {/* Error Overlay Modal */}
      {errorMessage && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4 backdrop-blur-xs">
          <div className="flex w-full max-w-xs flex-col items-center gap-5 rounded-3xl border-[3.5px] border-[#1E293B] bg-white p-6 shadow-[8px_8px_0px_0px_#1E293B]">
            <div className="flex flex-col items-center gap-1.5 text-center">
              <div className="flex h-11 w-11 items-center justify-center rounded-full border-[2px] border-[#1E293B] bg-[#EF4444] text-lg font-black text-white shadow-[2px_2px_0px_0px_#1E293B]">
                ✕
              </div>
              <h3 className="text-lg font-black text-[#1E293B]">Game Not Found</h3>
              <p className="text-xs font-bold text-slate-500">
                {errorMessage}
              </p>
            </div>

            <StylizedButton
              isSelected={true}
              text="Got it"
              color="bg-[#FACC15]"
              borderSize="medium"
              shadowSize="medium"
              textStyle="text-sm font-black tracking-wide"
              functionCallback={() => setErrorMessage(null)}
            />
          </div>
        </div>
      )}

      {IS_DEV_MODE && (
        <div className="fixed top-3 left-3 z-40 flex flex-wrap gap-1.5 rounded-lg border border-[#1E293B] bg-white p-1.5 shadow-md">
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