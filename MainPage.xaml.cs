using System.Globalization;

namespace CalculatriceMAUI;

public partial class MainPage : ContentPage
{
	// Nombre actuellement saisi/affiche (chaine, pour gerer proprement la virgule).
	private string _currentEntry = "0";

	// Valeur en attente (operande gauche) et operateur choisi.
	private double? _pendingValue = null;
	private string? _pendingOperator = null;

	// Vrai juste apres avoir appuye sur un operateur ou "=" : la prochaine touche
	// chiffre doit demarrer une nouvelle saisie plutot que s'ajouter a l'ancienne.
	private bool _startNewEntry = true;

	// Empeche d'enchainer les calculs quand une division par zero a ete signalee.
	private bool _hasError = false;

	private const int MaxDigits = 12;

	public MainPage()
	{
		InitializeComponent();
		RefreshDisplay();
	}

	// ------------------------------------------------------------------
	// Saisie des chiffres
	// ------------------------------------------------------------------
	private void OnDigitClicked(object sender, EventArgs e)
	{
		if (_hasError) ResetAll();

		var digit = ((Button)sender).Text;

		if (_startNewEntry)
		{
			_currentEntry = digit;
			_startNewEntry = false;
		}
		else
		{
			// Evite les "0" inutiles en tete et limite la longueur affichee.
			if (_currentEntry == "0")
				_currentEntry = digit;
			else if (_currentEntry.Replace("-", "").Replace(".", "").Length < MaxDigits)
				_currentEntry += digit;
		}

		RefreshDisplay();
	}

	private void OnDecimalClicked(object sender, EventArgs e)
	{
		if (_hasError) ResetAll();

		if (_startNewEntry)
		{
			_currentEntry = "0.";
			_startNewEntry = false;
		}
		else if (!_currentEntry.Contains('.'))
		{
			_currentEntry += ".";
		}

		RefreshDisplay();
	}

	// ------------------------------------------------------------------
	// Fonctions (AC, effacer dernier caractere, +/-, %)
	// ------------------------------------------------------------------
	private void OnClearAllClicked(object sender, EventArgs e)
	{
		ResetAll();
		RefreshDisplay();
	}

	private void OnBackspaceClicked(object sender, EventArgs e)
	{
		if (_hasError)
		{
			ResetAll();
			RefreshDisplay();
			return;
		}

		if (_startNewEntry) return; // rien a effacer sur un resultat fraichement calcule

		if (_currentEntry.Length <= 1 || (_currentEntry.Length == 2 && _currentEntry.StartsWith('-')))
			_currentEntry = "0";
		else
			_currentEntry = _currentEntry[..^1];

		RefreshDisplay();
	}

	private void OnSignClicked(object sender, EventArgs e)
	{
		if (_hasError) return;

		if (_currentEntry.StartsWith('-'))
			_currentEntry = _currentEntry[1..];
		else if (_currentEntry != "0")
			_currentEntry = "-" + _currentEntry;

		RefreshDisplay();
	}

	private void OnPercentClicked(object sender, EventArgs e)
	{
		if (_hasError) return;

		var value = ParseCurrentEntry();
		value /= 100.0;
		_currentEntry = FormatNumber(value);
		_startNewEntry = true;
		RefreshDisplay();
	}

	// ------------------------------------------------------------------
	// Operateurs (+, -, x, /) et signe "="
	// ------------------------------------------------------------------
	private void OnOperatorClicked(object sender, EventArgs e)
	{
		if (_hasError) return;

		var op = ((Button)sender).Text;

		// Si un calcul est deja en attente et que l'utilisateur enchaine un operateur
		// sans retaper de nombre, on calcule d'abord le resultat intermediaire.
		if (_pendingOperator != null && !_startNewEntry)
		{
			if (!TryComputePending()) return; // arret si division par zero
		}

		_pendingValue = ParseCurrentEntry();
		_pendingOperator = op;
		_startNewEntry = true;
		RefreshDisplay();
	}

	private void OnEqualsClicked(object sender, EventArgs e)
	{
		if (_hasError) return;
		if (_pendingOperator == null) return;

		if (TryComputePending())
		{
			_pendingOperator = null;
			_pendingValue = null;
			_startNewEntry = true;
			RefreshDisplay(clearOperationLine: true);
		}
	}

	/// <summary>
	/// Applique l'operateur en attente entre _pendingValue et la saisie courante.
	/// Retourne false (et affiche une erreur) en cas de division par zero.
	/// </summary>
	private bool TryComputePending()
	{
		double left = _pendingValue ?? 0;
		double right = ParseCurrentEntry();
		double result;

		switch (_pendingOperator)
		{
			case "+":
				result = left + right;
				break;
			case "−":
				result = left - right;
				break;
			case "×":
				result = left * right;
				break;
			case "÷":
				if (right == 0)
				{
					ShowDivisionByZeroError();
					return false;
				}
				result = left / right;
				break;
			default:
				result = right;
				break;
		}

		_currentEntry = FormatNumber(result);
		_pendingValue = result;
		return true;
	}

	private void ShowDivisionByZeroError()
	{
		_hasError = true;
		OperationLabel.Text = "Erreur";
		ResultLabel.Text = "Division par zéro";
		ResultLabel.FontSize = 26;
		_pendingOperator = null;
		_pendingValue = null;
		_startNewEntry = true;
	}

	// ------------------------------------------------------------------
	// Aides
	// ------------------------------------------------------------------
	private void ResetAll()
	{
		_currentEntry = "0";
		_pendingValue = null;
		_pendingOperator = null;
		_startNewEntry = true;
		_hasError = false;
		ResultLabel.FontSize = 52;
	}

	private double ParseCurrentEntry()
	{
		if (double.TryParse(_currentEntry, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
			return value;
		return 0;
	}

	private static string FormatNumber(double value)
	{
		// Supprime les zeros inutiles apres la virgule (ex: 4.50 -> 4.5, 4.00 -> 4).
		var text = value.ToString("0.##########", CultureInfo.InvariantCulture);
		return text.Length == 0 ? "0" : text;
	}

	private void RefreshDisplay(bool clearOperationLine = false)
	{
		ResultLabel.Text = _currentEntry;

		if (clearOperationLine || _pendingOperator == null)
		{
			OperationLabel.Text = clearOperationLine ? " " : " ";
		}

		if (_pendingOperator != null)
		{
			var leftText = FormatNumber(_pendingValue ?? 0);
			OperationLabel.Text = $"{leftText} {_pendingOperator}";
		}
	}
}
