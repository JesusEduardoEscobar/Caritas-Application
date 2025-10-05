import { useState, useEffect } from 'react';
import { AdminDashboard } from './components/AdminDashboard';
import { QRScanner } from './components/QRScanner';
import { LoginForm } from './components/auth/LoginForm';
import { AuthProvider, useAuth } from './components/auth/AuthProvider';
import { useDeviceDetection } from './hooks/useDeviceDetection';

function AppContent() {
  const { admin, isLoading } = useAuth();
  const { isMobile } = useDeviceDetection();
  const [showQRMode, setShowQRMode] = useState(false);

  // Auto-switch to QR mode on mobile devices
  useEffect(() => {
    if (isMobile && admin) {
      setShowQRMode(true);
    }
  }, [isMobile, admin]);

  // Show loading state
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary mx-auto mb-4"></div>
          <p className="text-muted-foreground">Cargando...</p>
        </div>
      </div>
    );
  }

  // Show login form if not authenticated
  if (!admin) {
    return <LoginForm />;
  }

  // Show QR scanner on mobile
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

  // Show admin dashboard
  return (
    <div className="min-h-screen bg-background">
      <AdminDashboard 
        onActivateQRMode={() => setShowQRMode(true)}
        isMobile={isMobile}
      />
    </div>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}