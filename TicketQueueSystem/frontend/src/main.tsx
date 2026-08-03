import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.tsx'
import { SignalRProvider } from './context/SingalRContext.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <SignalRProvider>
      <App />
    </SignalRProvider>
  </StrictMode>,
)
