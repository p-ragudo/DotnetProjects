import { useState, useRef, useEffect } from "react";
import StylizedButton from "./StylizedButton";
import StylizedText from "./StylizedText";

export type Tab = "create" | "join";

export interface LobbyContainerProps {
  setTab: (tab: Tab) => void;
  selectedTab: Tab;
  handleCreateRoom: () => void;
  handleJoinRoom: () => void;
}

export default function LobbyContainer({
  setTab,
  selectedTab,
  handleCreateRoom,
  handleJoinRoom,
}: LobbyContainerProps) {
  const [roomCode, setRoomCode] = useState("");
  const contentRef = useRef<HTMLDivElement>(null);
  const [contentHeight, setContentHeight] = useState<number | undefined>(undefined);

  useEffect(() => {
    if (!contentRef.current) return;

    const updateHeight = () => {
      if (contentRef.current) {
        // offsetHeight includes padding and borders accurately
        setContentHeight(contentRef.current.offsetHeight);
      }
    };

    const observer = new ResizeObserver(() => {
      updateHeight();
    });

    observer.observe(contentRef.current);
    updateHeight(); // Initial check

    return () => observer.disconnect();
  }, []);

  return (
    <div className="flex flex-col items-center gap-4">
      {/* Main Card Container with smooth height transition */}
      <div
        style={{
          height: contentHeight ? `${contentHeight}px` : "auto",
        }}
        className="w-full overflow-hidden rounded-4xl border-[3px] border-[#1E293B] bg-white shadow-[8px_8px_0px_0px_#1E293B] transition-[height] duration-200 ease-out"
      >
        {/* Measured Content Wrapper — padding goes HERE so offsetHeight captures everything */}
        <div ref={contentRef} className="p-8 pb-9">
          {/* Header Section */}
          <div className="mb-6 flex items-center gap-4">
            <div className="flex h-14 w-14 items-center justify-center rounded-2xl border-[2.5px] border-[#1E293B] bg-[#FACC15] shadow-[3px_3px_0px_0px_#1E293B]">
              <svg
                className="h-8 w-8 text-[#1E293B]"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2.5"
                strokeLinecap="round"
                strokeLinejoin="round"
              >
                <line x1="8" y1="3" x2="8" y2="21" />
                <line x1="16" y1="3" x2="16" y2="21" />
                <line x1="3" y1="8" x2="21" y2="8" />
                <line x1="3" y1="16" x2="21" y2="16" />
              </svg>
            </div>
            <div>
              <h1 className="text-2xl font-black tracking-tight text-[#1E293B]">
                Tic Tac Toe!
              </h1>
              <StylizedText 
                text="play a friend, anywhere"
                size="small"
                fontThickness="font-semibold"
                color="gray"
              />
            </div>
          </div>

          {/* Animated Tab Switcher Container */}
          <div className="relative mb-6 grid grid-cols-2 rounded-xl border-[2.5px] border-[#1E293B] bg-[#FFFBEA] p-1 shadow-[2px_2px_0px_0px_#1E293B]">
            {/* Sliding Background Pill */}
            <div
              className={`absolute bottom-1 top-1 w-[calc(50%-4px)] rounded-lg border-[2px] border-[#1E293B] bg-[#FACC15] shadow-[2px_2px_0px_0px_#1E293B] transition-transform duration-200 ease-out ${
                selectedTab === "create" ? "left-1 translate-x-0" : "left-1 translate-x-full"
              }`}
            />

            {/* Button 1 */}
            <StylizedButton
              functionCallback={() => setTab("create")}
              isSelected={false}
              textUnselectedColor={selectedTab === "create" ? "text-[#1E293B]" : "text-slate-600"}
              hasHover={false}
              text="Create room"
              customStyles="relative z-10 transition-colors duration-150"
            />

            {/* Button 2 */}
            <StylizedButton
              functionCallback={() => setTab("join")}
              isSelected={false}
              textUnselectedColor={selectedTab === "join" ? "text-[#1E293B]" : "text-slate-600"}
              hasHover={false}
              text="Join room"
              customStyles="relative z-10 transition-colors duration-150"
            />
          </div>

          <StylizedText
            text={
                selectedTab === "create"
                ? "Get a room code to send your opponent."
                : "Enter the room code shared by your opponent:"
            }
            size="small"
            color="semi-black"
            customStyles="text-center mb-3"
          />

          {/* Tab Content */}
          {selectedTab === "create" ? (
            <StylizedButton
              functionCallback={handleCreateRoom}
              isSelected={true}
              hasHover={true}
              text="Create room"
              textStyle="text-md font-bold"
              padding="py-3.5"
              borderSize="medium"
              shadowSize="medium"
            />
          ) : (
            <div>
              <input
                type="text"
                placeholder="e.g. AAY4Z"
                value={roomCode}
                onChange={(e) => setRoomCode(e.target.value.toUpperCase())}
                className="mb-6 w-full rounded-xl border-[2.5px] border-[#1E293B] bg-[#FFFBEA] p-3 text-center text-lg font-black uppercase tracking-widest text-[#1E293B] placeholder-slate-400 outline-none focus:ring-2 focus:ring-[#FACC15]"
              />
              <StylizedButton
                functionCallback={handleJoinRoom}
                isSelected={true}
                hasHover={true}
                text="Join room"
                textStyle="text-md font-bold"
                padding="py-3.5"
                borderSize="medium"
                shadowSize="medium"
              />
            </div>
          )}
        </div>
      </div>

      {/* Footer Text */}
      <p className="mt-3 text-xs font-semibold text-slate-400">
        No account needed — the room code is the only key.
      </p>
    </div>
  );
}