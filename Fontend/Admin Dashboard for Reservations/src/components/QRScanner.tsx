import { useState, useRef, useEffect } from 'react';
import { Button } from './ui/button';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { ArrowLeft, Camera, Scan } from 'lucide-react';
import QrScanner from 'qr-scanner';

interface QRScannerProps {
  onBack: () => void;
  onCodeScanned: (code: string) => void;
}

export function QRScanner({ onBack, onCodeScanned }: QRScannerProps) {
  const [isScanning, setIsScanning] = useState(false);
  const [lastScanned, setLastScanned] = useState<string>('');
  const videoRef = useRef<HTMLVideoElement>(null);
  const scannerRef = useRef<QrScanner | null>(null);

  const startCamera = async () => {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: 'environment' }
      });
      if (videoRef.current) {
        videoRef.current.srcObject = stream;
        videoRef.current.play();

        scannerRef.current = new QrScanner(
          videoRef.current,
          result => {
            if (result.data !== lastScanned) {
              setLastScanned(result.data);
              onCodeScanned(result.data);
            }
          },
          {
            returnDetailedScanResult: true,
            highlightScanRegion: true,
            highlightCodeOutline: true
          }
        );

        scannerRef.current.start();
        setIsScanning(true);
      }
    } catch (error) {
      console.error('Error accessing camera:', error);
    }
  };

  const stopCamera = () => {
    if (scannerRef.current) {
      scannerRef.current.stop();
      scannerRef.current.destroy();
    }
    if (videoRef.current?.srcObject) {
      const tracks = (videoRef.current.srcObject as MediaStream).getTracks();
      tracks.forEach(track => track.stop());
    }
    setIsScanning(false);
  };

  useEffect(() => {
    return () => stopCamera();
  }, []);

  return (
    <div className="min-h-screen bg-background p-4">
      <div className="max-w-md mx-auto">

        <Card className="mb-6">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Scan className="h-5 w-5 text-center" />
              Cámara
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="relative">
              <video
                ref={videoRef}
                className="w-full h-64 bg-muted rounded-lg object-cover"
                playsInline
                muted
              />
              <div className="absolute inset-0 border-2 border-primary rounded-lg opacity-50 pointer-events-none">
                <div className="absolute top-4 left-4 w-6 h-6 border-t-2 border-l-2 border-primary"></div>
                <div className="absolute top-4 right-4 w-6 h-6 border-t-2 border-r-2 border-primary"></div>
                <div className="absolute bottom-4 left-4 w-6 h-6 border-b-2 border-l-2 border-primary"></div>
                <div className="absolute bottom-4 right-4 w-6 h-6 border-b-2 border-r-2 border-primary"></div>
              </div>
            </div>
            
            <div className="flex gap-2 mt-4">
              {!isScanning ? (
                <Button onClick={startCamera} className="flex-1">
                  <Camera className="h-4 w-4 mr-2" />
                  Activar Cámara
                </Button>
              ) : (
                <>
                  <Button onClick={stopCamera} variant="secondary" className="flex-1">
                    Detener Cámara
                  </Button>
                </>
              )}
            </div>
          </CardContent>
        </Card>

        {lastScanned && (
          <Card>
            <CardHeader>
              <CardTitle>Último Código Escaneado</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="break-all bg-muted p-3 rounded">{lastScanned}</p>
              <p className="text-sm text-muted-foreground mt-2">
                Código enviado automáticamente a la computadora principal
              </p>
            </CardContent>
          </Card>
        )}

        <div className="mt-6 p-4 bg-accent rounded-lg">
          <h3 className="font-medium mb-2">Instrucciones:</h3>
          <ul className="text-sm space-y-1 text-muted-foreground">
            <li>• Apunta la cámara hacia el código QR</li>
            <li>• Mantén el código dentro del marco</li>
            <li>• El código se enviará automáticamente</li>
            <li>• Solo se confirman reservas de camas</li>
          </ul>
        </div>
      </div>
    </div>
  );
}