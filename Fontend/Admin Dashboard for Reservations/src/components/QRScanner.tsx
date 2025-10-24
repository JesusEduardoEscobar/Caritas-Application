import { useState, useRef, useEffect } from 'react';
import { Button } from './ui/button';
import { Card, CardContent, CardHeader, CardTitle} from './ui/card';
import { Badge } from './ui/badge';
import { ArrowLeft, Camera, Scan } from 'lucide-react';
import { API_URL } from '../Services/authLogin';
import axios from 'axios';
import { User, Reservation, ApiResponse, ReservationStatus} from '../types/models';
import QrScanner from 'qr-scanner';

const token = localStorage.getItem('token');


interface QRScannerProps {
  onBack: () => void;
  onCodeScanned: (code: string) => void;
}

interface ScannedObject {
  type: string;
  id: number;
  userId: number;
}

interface ReservationQR {
  id: number;
  shelter: string;
  bedNumber: string;
  startDate: string;
  endDate: string;

  userId: number;
  userName: string;
  email: string;
  phone: string;
}

export function QRScanner({ onBack, onCodeScanned }: QRScannerProps) {
  const [isScanning, setIsScanning] = useState(false);
  const [lastScanned, setLastScanned] = useState<string>('');
  const [scannedObject, setScannedObject] = useState<ScannedObject | null>(null);
  const [currentScannedObjectStatus, setCurrentScannedObjectStatus] = useState<ReservationStatus>('reserved');
  const [selectedScannedObjectStatus, setSelectedScannedObjectStatus] = useState<ReservationStatus>('reserved');
  const hasChanges = currentScannedObjectStatus !== selectedScannedObjectStatus;
  const statusOptions: { value: ReservationStatus; label: string }[] = [
    { value: "reserved", label: "Reservado" },
    { value: "checked_in", label: "Checked-in" },
    { value: "completed", label: "Completado" },
    { value: "canceled", label: "Cancelado" }
  ];

  const [reservation, setReservation] = useState<ReservationQR | null>(null);
  const [reservationError, setReservationError] = useState<string>('')
  
  const videoRef = useRef<HTMLVideoElement>(null);
  const scannerRef = useRef<QrScanner | null>(null);

  useEffect(() => {
    if(!lastScanned) return;

    const fetchData = async () => {
      try{
        const response = await axios.get(`${API_URL}/Qrs/${lastScanned}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        if (response.data.code === '200') {
          const obj: ScannedObject = response.data.data; 
          setScannedObject(obj);

          // console.log(obj);
        } else {
          setScannedObject(null);
        }
      } catch (err) {
        console.error(err);
        setScannedObject(null);
      }
    };

    fetchData();
  }, [lastScanned]);

  useEffect(() => {
    if(!scannedObject || !scannedObject.type || !scannedObject.id || !scannedObject.userId){
        setReservation(null);
        setReservationError("Reservacion no encontrada - QR inválido");
      return;
    }
      
    const getReservation = async () => {
      try {
        if (scannedObject.type === "Reservation") {
          const reservationResponse = await axios.get(`${API_URL}/Reservations/${scannedObject.id}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
          });
          if(reservationResponse.data.code !== '200'){
            setReservation(null);
            setReservationError("Reservacion no encontrada");
            return;
          }
          const reservationData = reservationResponse.data.data;

          const bedResponse = await axios.get(`${API_URL}/Beds/${reservationData.bedId}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
          });
          if (bedResponse.data.code !== '200') {
            setReservation(null);
            setReservationError("Cama no encontrada");
            return;
          }
          const bedData = bedResponse.data.data;

          const shelterResponse = await axios.get(`${API_URL}/Shelters/${bedData.shelterId}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
          });
          if (shelterResponse.data.code !== '200') {
            setReservation(null);
            setReservationError("Posada no encontrada");
            return;
          }
          const shelterData = shelterResponse.data.data;

          // Check user id consistency
          const userResponse = await axios.get(`${API_URL}/Users/${reservationData.userId}`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
          });
          if (userResponse.data.code !== '200') {
            setReservation(null);
            setReservationError("Usuario no encontrado");
            return;
          }
          const userData = userResponse.data.data[0];

          if (userData.id != scannedObject.userId){
            setReservation(null);
            setReservationError("El usuario no coincide con el de la reservación");
            return;
          }

          const reservationQR: ReservationQR = {
            id: scannedObject.id,
            shelter: shelterData.name,
            bedNumber: bedData.bedNumber,
            startDate: reservationData.startDate,
            endDate: reservationData.endDate,

            userId: userData.id,
            userName: userData.name,
            email: userData.email,
            phone: userData.phone
          };

          setReservation(reservationQR);
          setCurrentScannedObjectStatus(reservationData.status as ReservationStatus);
          setSelectedScannedObjectStatus(reservationData.status as ReservationStatus);
          setReservationError('');
        } else {
          setReservation(null);
          setReservationError("Tipo de reservacion no soportado");
        }
      } catch (err) {
          console.error(err);
      }
    };

    getReservation();
  }, [scannedObject]);

  const formatDate = (dateString: string) => {
    return dateString.replace('T', ' ').split('.')[0];
  };

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
          <div className="space-y-4">
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

            {reservation ? (
              <Card>
                <CardHeader>
                  <CardTitle>Reservación #{reservation.id}</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-gray-900 font-medium">
                    {reservation.shelter}
                  </div>
                  <Badge variant={"outline"}>Cama: {reservation.id}</Badge>
                  <div className="flex gap-4 text-sm text-gray-600">
                    <div>
                      <span className="font-medium">Inicio:</span> {formatDate(reservation.startDate)}
                    </div>
                    <div>
                      <span className="font-medium">Fin:</span> {formatDate(reservation.endDate)}
                    </div>
                  </div>

                  <div className="space-y-2">
                    <label className="block text-sm font-medium text-gray-700">
                      Status
                    </label>
                    <select
                      value={selectedScannedObjectStatus}
                      onChange={(e) => setSelectedScannedObjectStatus(e.target.value as ReservationStatus)}
                      className="w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                    >
                      {statusOptions.map((option) => (
                        <option key={option.value} value={option.value}>
                          {option.label}
                        </option>
                      ))}
                    </select>
                  </div>

                  {hasChanges && (
                    <button
                      onClick={() => {
                        console.log('Guardando cambios...', { newStatus: selectedScannedObjectStatus });
                        
                        const saveStatus = async () => {
                          try {
                            const response = await axios.patch(`${API_URL}/Reservations/status`,
                              { id: reservation.id, status: selectedScannedObjectStatus },
                            {
                              headers: {
                                  Authorization: `Bearer ${token}`
                              }
                            });
                            if(response.data.code === '200'){
                              setCurrentScannedObjectStatus(selectedScannedObjectStatus);
                            } else {
                              
                            }
                          } catch (err) {
                            console.log(err)
                          }
                        };
                        
                        saveStatus();
                      }}
                      className="mt-100 inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-all disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 shrink-0 [&_svg]:shrink-0 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive bg-primary text-primary-foreground hover:bg-primary/90 h-10 px-6 py-3 has-[>svg]:px-4">
                      Guardar Cambios
                    </button>
                  )}
                </CardContent>
                <CardHeader>
                  <CardTitle>Usuario #{reservation.userId}</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-gray-900 font-medium">
                    {reservation.userName}
                  </div>
                  <div className='flex gap-4 text-sm text-gray-600'>
                    <div>
                      <span className="font-medium">Correo:</span> {reservation.email}
                    </div>
                    <div>
                      <span className="font-medium">Teléfono:</span> {reservation.phone}
                    </div>
                  </div>
                </CardContent>
              </Card>
            ) : (
              <div className="p-6 text-center text-red-600 bg-red-50 border border-red-200 rounded-lg">
                {reservationError != "" ?(
                    reservationError
                ) : (
                  "No se encontró ninguna reservación o hubo un error al cargar los datos."
                )}
              </div>
            )}
          </div>
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