import React from 'react';

export interface ResponseModalProps {
  isOpen: boolean;
  isSuccess: boolean;
  message: string;
  onClose: () => void;
}

export const ResponseModal: React.FC<ResponseModalProps> = ({
  isOpen,
  isSuccess,
  message,
  onClose,
}) => {
  if (!isOpen) {
    return null;
  }

  return (
    <div className="modal-overlay" onClick={onClose} role="dialog" aria-modal="true">
      <div className={`modal-content modal-${isSuccess ? 'success' : 'error'}`} onClick={(e) => e.stopPropagation()}>
        <div className="modal-icon">{isSuccess ? '✓' : '✕'}</div>
        <h2>{isSuccess ? '¡Éxito!' : 'Error'}</h2>
        <p>{message}</p>
        <div className="modal-actions">
          <button type="button" className="btn-primary" onClick={onClose}>
            Aceptar
          </button>
        </div>
      </div>
    </div>
  );
};
