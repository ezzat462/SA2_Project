import React, { useState, useEffect, useContext } from 'react';
import licenseService from '../services/licenseService';
import { ThemeContext } from '../context/ThemeContext';
import { toast } from 'react-toastify';

export default function LicensePage() {
    const { theme } = useContext(ThemeContext);
    const [status, setStatus] = useState(null);
    const [file, setFile] = useState(null);
    const [loading, setLoading] = useState(true);
    const [message, setMessage] = useState('');

    useEffect(() => {
        fetchLicenseStatus();

        const handleUpdate = (e) => {
            if (e.detail?.licenseStatus) {
                setStatus(e.detail.licenseStatus);
            }
        };

        window.addEventListener('USER_STATUS_UPDATED', handleUpdate);
        return () => window.removeEventListener('USER_STATUS_UPDATED', handleUpdate);
    }, []);

    const fetchLicenseStatus = async () => {
        try {
            const data = await licenseService.getMyLicense();
            if (data.data) {
                setStatus(data.data.status);
            }
        } catch (err) {
            console.error('Error fetching license status', err);
        } finally {
            setLoading(false);
        }
    };

    const handleUpload = async (e) => {
        e.preventDefault();
        if (!file) return;

        try {
            setLoading(true);
            const res = await licenseService.uploadLicense(file);
            toast.success('License uploaded successfully! Admins have been notified.');
            await fetchLicenseStatus();
        } catch (err) {
            toast.error(err.response?.data?.message || 'Error uploading license.');
            setLoading(false);
        }
    };

    if (loading) return <div>Loading...</div>;

    const renderContent = () => {
        if (status === 'Pending') {
            return (
                <div className="alert alert-info">
                    <h5>⏳ Your license is under review</h5>
                    <p>We will notify you once it is verified.</p>
                </div>
            );
        }
        
        if (status === 'Verified') {
            return (
                <div className="alert alert-success">
                    <h5>✅ License Verified</h5>
                    <p>You can now book cars on DriveShare.</p>
                </div>
            );
        }

        return (
            <div>
                {status === 'Rejected' && (
                    <div className="alert alert-danger">
                        Your previous document was rejected. Please re-upload a clear image of your driver's license.
                    </div>
                )}
                <form onSubmit={handleUpload}>
                    <div className="mb-3">
                        <label className="form-label text-start d-block">Upload Driver License Image</label>
                        <input 
                            type="file" 
                            className="form-control" 
                            accept="image/*"
                            onChange={(e) => setFile(e.target.files[0])} 
                            required 
                        />
                    </div>
                    <button 
                        type="submit" 
                        className="btn w-100 placeholder-wave"
                        style={{ background: "linear-gradient(45deg, #6a0dad, #9b30ff)", color: "white", border: "none" }}
                        disabled={loading}
                    >
                        Submit Document
                    </button>
                </form>
                {message && <div className="mt-3 alert alert-info">{message}</div>}
            </div>
        );
    };

    return (
        <div className="container mt-5" style={{ maxWidth: '600px' }}>
            <div 
                className={`card shadow-sm p-4 ${theme === 'dark' ? 'bg-dark text-white' : ''}`}
                style={{ borderRadius: "15px", backdropFilter: "blur(10px)", background: theme === 'dark' ? 'rgba(33, 37, 41, 0.8)' : 'rgba(255, 255, 255, 0.8)' }}
            >
                <h2 className="mb-4 text-center">Driver License Verification</h2>
                {renderContent()}
            </div>
        </div>
    );
}
