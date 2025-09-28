import { useState, useEffect } from 'react';
import { AdminDashboard } from './components/AdminDashboard';
import { QRScanner } from './components/QRScanner';
import { useDeviceDetection } from './hooks/useDeviceDetection';

export default function App() {
  const { isMobile } = useDeviceDetection();
  const [showQRMode, setShowQRMode] = useState(false);

  // Auto-switch to QR mode on mobile devices
  useEffect(() => {
    if (isMobile) {
      setShowQRMode(true);
    }
  }, [isMobile]);

  if (showQRMode && isMobile) {
    return (
      <QRScanner 
        onBack={() => setShowQRMode(false)}
        onCodeScanned={(code) => {
          // In real implementation, this would send to connected computer
          console.log('QR Code escaneado:', code);
        }}
      />
    );
  }

  return (
    <div className="min-h-screen bg-background">
      <AdminDashboard 
        onActivateQRMode={() => setShowQRMode(true)}
        isMobile={isMobile}
      />
    </div>
  );
}