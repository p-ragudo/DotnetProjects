import React from 'react'

interface GameLayoutProps {
  children: React.ReactNode
}

export function GameLayout({ children }: GameLayoutProps) {
  return (
    <div className="relative min-h-screen w-full bg-[#FFFBEA] bg-[radial-gradient(#E8D2A7_2px,transparent_2px)] bg-size-[[24px_24px] flex flex-col items-center justify-center p-4">
      {/* Optional: Floating decorative container/wrapper */}
      <main className="w-full max-w-md">
        {children}
      </main>
    </div>
  )
}